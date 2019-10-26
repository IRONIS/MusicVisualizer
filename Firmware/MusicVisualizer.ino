int i = 0;  // переменная для счетчика имитирующего показания датчика
int led = 13;

#define NUM_LEDS 100            // количество светодиодов
#define DI_PIN 6                // пин, к которому подключена лента
#define start_flashes 1         // проверка цветов при запуске (1 - включить, 0 - выключить)

byte EMPTY_BRIGHT = 30;         // яркость "не горящих" светодиодов (0 - 255)
#define EMPTY_COLOR HUE_PURPLE  // цвет "не горящих" светодиодов. Будет чёрный, если яркость 0
#define BRIGHTNESS 200          // яркость (0 - 255)

// настройки радуги
#define RAINBOW_SPEED 6    // скорость движения радуги (чем меньше число, тем быстрее радуга)
#define RAINBOW_STEP 6     // шаг изменения цвета радуги

#define EXP 1.4
#define MODE 0              // режим при запуске
#define MAIN_LOOP 5         // период основного цикла отрисовки (по умолчанию 5)
#define SMOOTH 0.5          // коэффициент плавности анимации VU (по умолчанию 0.5)
#define SMOOTH_FREQ 0.8     // коэффициент плавности анимации частот (по умолчанию 0.8)
#define MAX_COEF 1.8
uint16_t LOW_PASS = 100;         // нижний порог шумов режим VU, ручная настройка

const byte numChars = 32;

char receivedChars[numChars];
char tempChars[numChars];

boolean newData = false;

byte  Rlenght, Llenght;
float RsoundLevel, RsoundLevel_f;
float LsoundLevel, LsoundLevel_f;



float averK = 0.006, k = SMOOTH, k_freq = SMOOTH_FREQ;
float averageLevel = 50;
int hue;
int maxLevel = 100;
byte MAX_CH = NUM_LEDS / 2;
float index = (float)255 / MAX_CH;   // коэффициент перевода для палитры
byte count;

unsigned long main_timer, hue_timer, rainbow_timer;

#include <FastLED.h>

// градиент-палитра от зелёного к красному
DEFINE_GRADIENT_PALETTE(soundlevel_gp) {
  0,    0,    255,  0,  // green
  100,  255,  255,  0,  // yellow
  150,  255,  100,  0,  // orange
  200,  255,  50,   0,  // red
  255,  255,  0,    0   // red
};
CRGBPalette32 myPal = soundlevel_gp;

CRGB leds[NUM_LEDS];  // создаём ленту

void setup() {
  Serial.begin(9600);    // установим скорость обмена данными
  pinMode(led, OUTPUT);  // и режим работы 13-ого цифрового пина в качестве выхода

  FastLED.addLeds<WS2812, DI_PIN, GRB>(leds, NUM_LEDS).setCorrection( TypicalLEDStrip );
  FastLED.setBrightness(BRIGHTNESS);  // инициализация светодиодов

  // вспышки красным синим и зелёным при запуске (можно отключить)
  if (start_flashes) {
    LEDS.showColor(CRGB(255, 0, 0));
    delay(500);
    LEDS.showColor(CRGB(0, 255, 0));
    delay(500);
    LEDS.showColor(CRGB(0, 0, 255));
    delay(500);
    LEDS.showColor(CRGB(0, 0, 0));
  }

  Serial.println("ArduinoHandShake");  // отправляем строку на порт
}


void loop() {
  if (millis() - main_timer > MAIN_LOOP) {
    // сбрасываем значения
    RsoundLevel = 0;
    LsoundLevel = 0;

    // Parse data
    recvWithStartEndMarkers();
    if (newData == true) {
      newData = false;

      strcpy(tempChars, receivedChars);
      parseData();
    }

    // фильтруем по нижнему порогу шумов
    RsoundLevel = map(RsoundLevel, LOW_PASS, 1023, 0, 500);
    LsoundLevel = map(LsoundLevel, LOW_PASS, 1023, 0, 500);

    // ограничиваем диапазон
    RsoundLevel = constrain(RsoundLevel, 0, 500);
    LsoundLevel = constrain(LsoundLevel, 0, 500);

    // возводим в степень (для большей чёткости работы)
    RsoundLevel = pow(RsoundLevel, EXP);
    LsoundLevel = pow(LsoundLevel, EXP);

    // фильтр
    RsoundLevel_f = RsoundLevel * SMOOTH + RsoundLevel_f * (1 - SMOOTH);
    LsoundLevel_f = LsoundLevel * SMOOTH + LsoundLevel_f * (1 - SMOOTH);

    LsoundLevel_f = RsoundLevel_f;  // если моно, то левый = правому

    // заливаем "подложку", если яркость достаточная
    if (EMPTY_BRIGHT > 5) {
      for (int i = 0; i < NUM_LEDS; i++)
        leds[i] = CHSV(EMPTY_COLOR, 255, EMPTY_BRIGHT);
    }

    // если значение выше порога - начинаем самое интересное
    if (RsoundLevel_f > 15 && LsoundLevel_f > 15) {

      // расчёт общей средней громкости с обоих каналов, фильтрация.
      // Фильтр очень медленный, сделано специально для автогромкости
      averageLevel = (float)(RsoundLevel_f + LsoundLevel_f) / 2 * averK + averageLevel * (1 - averK);

      // принимаем максимальную громкость шкалы как среднюю, умноженную на некоторый коэффициент MAX_COEF
      maxLevel = (float)averageLevel * MAX_COEF;

      // преобразуем сигнал в длину ленты (где MAX_CH это половина количества светодиодов)
      Rlenght = map(RsoundLevel_f, 0, maxLevel, 0, MAX_CH);
      Llenght = map(LsoundLevel_f, 0, maxLevel, 0, MAX_CH);

      // ограничиваем до макс. числа светодиодов
      Rlenght = constrain(Rlenght, 0, MAX_CH);
      Llenght = constrain(Llenght, 0, MAX_CH);

      animation();       // отрисовать
    }

    FastLED.show();           // отправить значения на ленту
    FastLED.clear();          // очистить массив пикселей
  }
}

void recvWithStartEndMarkers() {
  static boolean recvInProgress = false;
  static byte ndx = 0;
  char startMarker = '<';
  char endMarker = '>';
  char rc;

  while (Serial.available() > 0 && newData == false) {
    rc = Serial.read();
    if (recvInProgress == true) {
      if (rc != endMarker) {
        receivedChars[ndx] = rc;
        ndx++;
        if (ndx >= numChars) {
          ndx = numChars - 1;
        }
      }
      else {
        receivedChars[ndx] = '\0'; // terminate the string
        recvInProgress = false;
        ndx = 0;
        newData = true;
      }
    }
    else if (rc == startMarker) {
      recvInProgress = true;
    }
  }
}

void parseData() {      // split the data into its parts

  char * strtokIndx; // this is used by strtok() as an index

  strtokIndx = strtok(tempChars, ","); // this continues where the previous call left off
  LsoundLevel = atof(strtokIndx);     // convert this part to an integer

  strtokIndx = strtok(NULL, ",");
  RsoundLevel = atof(strtokIndx);     // convert this part to a float
}


void animation() {
  if (millis() - rainbow_timer > 30) {
    rainbow_timer = millis();
    hue = floor((float)hue + RAINBOW_STEP);
  }
  count = 0;
  for (int i = (MAX_CH - 1); i > ((MAX_CH - 1) - Rlenght); i--) {
    leds[i] = ColorFromPalette(RainbowColors_p, (count * index) / 2 - hue);  // заливка по палитре радуга
    count++;
  }
  count = 0;
  for (int i = (MAX_CH); i < (MAX_CH + Llenght); i++ ) {
    leds[i] = ColorFromPalette(RainbowColors_p, (count * index) / 2 - hue); // заливка по палитре радуга
    count++;
  }
  if (EMPTY_BRIGHT > 0) {
    CHSV this_dark = CHSV(EMPTY_COLOR, 255, EMPTY_BRIGHT);
    for (int i = ((MAX_CH - 1) - Rlenght); i > 0; i--)
      leds[i] = this_dark;
    for (int i = MAX_CH + Llenght; i < NUM_LEDS; i++)
      leds[i] = this_dark;
  }
}
