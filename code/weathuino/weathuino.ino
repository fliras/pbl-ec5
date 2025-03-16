/* TODO: 
- Implementar a realização de medições e testes em função do período de leitura nas configurações;

Testes a realizar:
- Testar novas funcionalidades que forem implementadas;
*/

#include <avr/eeprom.h>
#include <EEPROM.h>
#include <LiquidCrystal_I2C.h>
#include <DHT.h>
#include <RTClib.h>
#include <Wire.h>
#include <Arduino.h>


#define DHTTYPE DHT22 // No projeto físico trocar para DHT11
#define SENSOR_TEMPERATURA_UMIDADE A1

#define LDR A0 // pino LDR

#define BUZZER 11 // Pino Buzzer

//pinos do led rgb
#define LED_VERMELHO 10
#define LED_VERDE 9
#define LED_AZUL 8

//pinos dos botões
#define BOTAO_E 5
#define BOTAO_M 6
#define BOTAO_D 7

//Códigos de erro das leituras e problemas dos sensores
#define COD_ERRO_SENSOR_TEMPERATURA_MEDIDA 1
#define COD_ERRO_SENSOR_UMIDADE_MEDIDA 2
#define COD_ERRO_SENSOR_LUMINOSIDADE_MEDIDA 3
#define COD_ERRO_SENSOR_DHT_FALHA 4

// Endereço inicial em que os dados do erro serão registrados
#define ENDERECO_INICIAL_REGISTROS_NA_EEPROM 19

// Endereço inicial das configurações
#define ENDERECO_INICIAL_CONFIGURACOES 1

//Configurações de fábrica
#define LIMITE_MINIMO_LDR 0
#define LIMITE_MAXIMO_LDR 1023
#define LIMITE_MINIMO_SENSOR_TEMPERATURA_INICIAL 15
#define LIMITE_MAXIMO_SENSOR_TEMPERATURA_INICIAL 25
#define LIMITE_MINIMO_SENSOR_UMIDADE_INICIAL 30
#define LIMITE_MAXIMO_SENSOR_UMIDADE_INICIAL 50
#define LIMITE_MINIMO_SENSOR_LUMINOSIDADE_INICIAL 0
#define LIMITE_MAXIMO_SENSOR_LUMINOSIDADE_INICIAL 30
#define IDIOMA_INICIAL 1
#define ESTADO_BUZZER_INICIAL 1
#define TIPO_SOM_BUZZER_INICIAL 1
#define PERIODO_MEDICAO_INICIAL 1
#define FUSO_HORARIO_ATUAL -3
#define UNIDADE_TEMPERATURA_INICIAL 1

//Variáveis para manipular e usar os dados obtidos dos sensores e outros componentes
LiquidCrystal_I2C lcd(0x27, 16, 2);
RTC_DS1307 rtc;
DateTime timestampAtual;
DHT dht(SENSOR_TEMPERATURA_UMIDADE, DHTTYPE);

// Valores a serem medidos pelos sensores
int temperatura = 0.0;
unsigned int umidade = 0;
unsigned int luminosidade = 0;

//Condições de irregularidade nas medidas caso sejam maiores ou menores que os limites
bool temperaturaIrregular = false;
bool umidadeIrregular = false;
bool luminosidadeIrregular = false;

// Estrutura para os dados de log dos erros
struct DadosFalha{
  DateTime dataHora;
  uint16_t erro;
};

// Estrutura para configurações do sistema
struct Configuracoes{
  unsigned int minLDR;
  unsigned int maxLDR;
  byte minTemperatura;
  byte maxTemperatura;
  byte minUmidade;
  byte maxUmidade;
  byte minLuminosidade;
  byte maxLuminosidade;
  bool estadoBuzzer;
  byte tipoSomBuzzer;
  unsigned int periodoMedicao;
  byte idioma;
  int fusoHorario;
  byte unidadeTemperatura;
};



// funções para retornar se os valores estão dentro ou fora do limite
bool valorEstaIrregular(bool temperaturaIrregular, bool umidadeIrregular, bool luminosidadeIrregular);
bool temperaturaForaLimite(float temperaturaMedida);
bool umidadeForaLimite(float umidadeMedida);
bool luminosidadeForaLimite(unsigned int luminosidadeMedida);

String parametroMostradoNoDisplay[] = { "Temp", "Lumi", "Umid" };
short idParametroDisplay = 0;

String montarStringTimeStamp(DateTime dataHora); // monta string do timestamp para uso em alguma saída visual

void acionaBuzzer(); // aciona o buzzer em som contínuo
void desligaBuzzer(); // desliga o buzzer


// funções para acionamento de uma das cores do led rgb
void acionaSinalVermelho();
void acionaSinalAmarelo();
void acionaSinalVerde();

// Mostra as configurações locais na serial
void mostarConfiguracoesLocal(Configuracoes configs);


//Funções para buscar dados importantes na EEPROM
void mostarConfiguracoesEEPROM();
void mostrarDadosNaSerial();
void mostrarDadosFalhaSalvos();
DadosFalha resgataDadosFalhaEEPROMPorIndice(int indice);
DadosFalha resgataDadosFalhaEEPROMPorEndereco(int endereco);
int encontraEnderecoPorIndice(int indice);
Configuracoes resgataConfiguracoesNaEEPROM();

// Funções para gravação da EEPROM
void gravarDadosFalhaNaEEPROM(byte codFalha, byte medida, DateTime timestamp);
void gravarConfiguracoesNaEEPROM(Configuracoes configs);
uint16_t montarCodigoErroEmUInt16(byte codFalhaComponente, byte medida);

void atualizarMedidas(); // atualiza as variáveis de temperatura, umidade e luminosidade
double converterCelsiusParaFahrenheit(double temperaturaCelsius);
double converterCelsiusParaKelvin(double temperaturaCelsius);

void voltarParaConfiguracoesDeFabrica();

void limparRegistrosFalhaEEPROM(); //deixa todos os bytes da memória EEPROM em 0 e coloca o ultimo endereço livre no inicial

//Configurações do sistema
Configuracoes config = resgataConfiguracoesNaEEPROM();




//Dados de menu
/*
1) Definir o range do sensor LDR (ausência de luz e luminosidade total)
2) Definir limites de temperatura, umidade e luminosidade com os quais a placa irá trabalhar
3) ativar/desativar alertas sonoros
4) exibir registros de anormalidade
*/
enum MENU_STATE{
  MAIN,
  SETLDR,
  SETSOUND,
  SETTEMPMAX,
  SETTEMPMIN,
  SETHUMIMAX,
  SETHUMIMIN,
  SETLUMIMAX,
  SETLUMIMIN,
  GETREGISTRY
  };

enum BUTTON_TYPE{BUTTON_LEFT,BUTTON_MIDDLE,BUTTON_RIGHT};
//LEFT    -> butao da esquerda( Escape )
//MIDDLE  -> Butao do meio    (   <-   )
//RIGHT   -> Butao da direita (   ->   )

MENU_STATE current_menu = SETLDR;//O menu atual
int main_menu_index = 05;//A opção selecionada quando estamo no menu principal.

byte iconeWeathino1[] = {
  B00000, B10000, B11000, B11000,
  B11011, B11011, B01011, B00001
};

byte iconeWeathino2[] = {
  B00001, B01011, B11011, B11011,
  B11000, B11000, B10000, B00000
};

byte iconeWeathino3[] = {
  B00111, B00111, B00011, B00011,
  B00001, B00000, B00000, B00000
};

byte iconeWeathino4[] = {
  B00000, B00000, B00000, B00001,
  B00011, B00011, B00111, B00111
};

byte iconeWeathino5[] = {
  B01010, B01010, B01010, B01010,
  B11011, B10001, B11011, B01110
};

byte iconeWeathino6[] = {
  B00100, B01010, B01010, B01010,
  B01010, B01010, B01010, B01010
};


void setup() {
  limparRegistrosFalhaEEPROM();

  voltarParaConfiguracoesDeFabrica();


  Serial.begin(9600);

  pinMode(LDR, INPUT);
  pinMode(SENSOR_TEMPERATURA_UMIDADE, INPUT);
  pinMode(LED_VERMELHO, OUTPUT);
  pinMode(LED_AZUL, OUTPUT);
  pinMode(LED_VERDE, OUTPUT);
  pinMode(BUZZER, OUTPUT);
  pinMode(BOTAO_E, INPUT);
  pinMode(BOTAO_M, INPUT);
  pinMode(BOTAO_D, INPUT);

  //Inicializa os componentes
  EEPROM.begin();
  lcd.begin(16, 2);
  lcd.backlight();
  dht.begin();
  if(!rtc.begin())
    Serial.println("RTC não inicializou");

  rtc.adjust(DateTime(F(__DATE__), F(__TIME__))); // ajusta o rtc para o horário atual da gravação
  inicializaMenu();
}

void loop() {

  timestampAtual = rtc.now();
  atualizarMedidas();
  atualizaMedidasNoDisplay();

  int offsetSeconds = config.fusoHorario * 3600; // pega as horas em seegundos
  timestampAtual = timestampAtual.unixtime() + offsetSeconds; // Adicionando o deslocamento ao tempo atual

  //mostrarDadosNaSerial();
  Serial.println(digitalRead(BOTAO_E));
  Serial.println(digitalRead(BOTAO_M));
  Serial.println(digitalRead(BOTAO_D));
  Serial.println("");

  if (isnan(temperatura) || isnan(umidade)) // verifica se os valores do sensor dht são válidos e grava na eeprom o erro se não estiverem
  {
    gravarDadosFalhaNaEEPROM(COD_ERRO_SENSOR_DHT_FALHA, 0, timestampAtual);
    acionaBuzzer();
  } 
  else{
    desligaBuzzer();
  }

  // atualiza a verificação dos estados das medidas
  temperaturaIrregular = temperaturaForaLimite(temperatura);
  umidadeIrregular = umidadeForaLimite(umidade);
  luminosidadeIrregular = luminosidadeForaLimite(luminosidade);
  
  // grava erro caso algum dado esteja fora do padrão
  if(valorEstaIrregular(temperaturaIrregular, umidadeIrregular, luminosidadeIrregular)){

    if(temperaturaIrregular)
      gravarDadosFalhaNaEEPROM(COD_ERRO_SENSOR_TEMPERATURA_MEDIDA, temperatura, timestampAtual);
    if(umidadeIrregular)
      gravarDadosFalhaNaEEPROM(COD_ERRO_SENSOR_UMIDADE_MEDIDA, umidade, timestampAtual);
    if(luminosidadeIrregular)
      gravarDadosFalhaNaEEPROM(COD_ERRO_SENSOR_LUMINOSIDADE_MEDIDA, luminosidade, timestampAtual);

    acionaSinalVermelho();
    acionaBuzzer();

  }
  else{
    acionaSinalVerde();
    desligaBuzzer();
  }
  delay(2000);
}

void atualizaMedidasNoDisplay() {
  String dados = padRight(" " + String(temperatura) + "C", 4) + "  " +
    padLeft(String(umidade) + "%", 4) + " " +
    padLeft(String(luminosidade) + "%", 4);

  exibeTextoNoLCD(" TEMP UMID LUMI", dados);
}

String padLeft(String texto, int tamanho) {
  String novaString = texto;
  while (novaString.length() < tamanho)
    novaString = " " + novaString;
  return novaString;
}

String padRight(String texto, int tamanho) {
  String novaString = texto;
  while (novaString.length() < tamanho)
    novaString = novaString + " ";
  return novaString;
}

bool valorEstaIrregular(bool temperaturaIrregular, bool umidadeIrregular, bool luminosidadeIrregular){
  if(temperaturaIrregular || umidadeIrregular || luminosidadeIrregular)
    return true;
  return false;
}

bool temperaturaForaLimite(float temperaturaMedida){
  if(temperaturaMedida < config.minTemperatura || temperaturaMedida > config.maxTemperatura)
      return true;
    return false;
}

bool umidadeForaLimite(unsigned int umidadeMedida){
  if(umidadeMedida < config.minUmidade || umidadeMedida > config.maxUmidade)
      return true;
    return false;
}

bool luminosidadeForaLimite(unsigned int luminosidadeMedida){
  if(luminosidadeMedida < config.minLuminosidade || luminosidadeMedida > config.maxLuminosidade)
      return true;
    return false;
}

String montarStringTimeStamp(DateTime dataHora){
  return montaStringData(dataHora) + " " + montaStringHora(dataHora);
}

String montaStringData(DateTime dataHora) {
  String dia = "";
  String mes = "";

  if(dataHora.day() < 10)
    dia = String(dia + String(0));
  if(dataHora.month() < 10)
    mes = String(mes + String(0));

  dia = String(dia + String(dataHora.day()));
  mes = String(mes + String(dataHora.month()));

  return String(dia + "/" + mes + "/" + String(dataHora.year()));
}

String montaStringHora(DateTime dataHora) {
  String hora = "";
  String minuto = "";
  String segundo = "";

  if(dataHora.hour() < 10)
    hora = String(hora + String(0));
  if(dataHora.minute() < 10)
    minuto = String(minuto + String(0));
  if(dataHora.second() < 10)
    segundo = String(segundo + String(0));

  hora = String(hora + String(dataHora.hour()));
  minuto = String(minuto + String(dataHora.minute()));
  segundo = String(segundo + String(dataHora.second()));

  return String(hora + ":" + minuto + ":" + segundo);
}

void acionaBuzzer(){
  tone(BUZZER, 600);
}

void desligaBuzzer(){
  noTone(BUZZER); 
}


// TODO Corrigir
/*
void alarmeTipo1(){
  unsigned long millisAnterior = millis();
      tone(BUZZER,600);
  for(int i = 0; i < 10; i++){

    if (millis() - millisAnterior > 5000) {
      tone(BUZZER,600);
    }

    if(millis() - millisAnterior > 10000){
      tone(BUZZER, 900);
      millisAnterior = millis();
    }
  }
}

void alarmeTipo2(){
  unsigned long millisAnterior = millis();
  tone(BUZZER, 600);
  for(int i = 0; i < 10; i++){

    if (millis() - millisAnterior > 500) {
      tone(BUZZER, 200);
    }

    if(millis() - millisAnterior > 1000){
      tone(BUZZER, 600);
      millisAnterior = millis();
    }
  }
}

void alarmeTipo3(){
  unsigned long millisAnterior = millis();
  tone(BUZZER, 600);
  for(int i = 0; i < 10; i++){

    if (millis() - millisAnterior > 250) {
      tone(BUZZER, 200);
    }

    if(millis() - millisAnterior > 500){
      tone(BUZZER, 600);
      millisAnterior = millis();
    }
  }
}
// Corrigir até aqui]
*/

void acionaSinalVermelho(){
    analogWrite(LED_VERMELHO, 255);
    analogWrite(LED_VERDE, 0);
    analogWrite(LED_AZUL, 0);
}

void acionaSinalAmarelo(){
    analogWrite(LED_VERMELHO, 255);
    analogWrite(LED_VERDE, 255);
    analogWrite(LED_AZUL, 0);
}

void acionaSinalVerde(){
    analogWrite(LED_VERMELHO, 0);
    analogWrite(LED_VERDE, 255);
    analogWrite(LED_AZUL, 0);
}

void mostarConfiguracoesLocal(Configuracoes configs){

  Serial.print("Minimo valor LDR: ");
  Serial.println(configs.minLDR);
  Serial.print("Máximo valor LDR: ");
  Serial.println(configs.maxLDR);
  Serial.print("Minimo valor Temperatura: ");
  Serial.println(configs.minTemperatura);
  Serial.print("Máximo valor Temperatura: ");
  Serial.println(configs.maxTemperatura);
  Serial.print("Mínimo valor Umidade: ");
  Serial.println(configs.minUmidade);
  Serial.print("Máximo valor Umidade: ");
  Serial.println(configs.maxUmidade);
  Serial.print("Mínimo valor Luminosidade: ");
  Serial.println(configs.minLuminosidade);
  Serial.print("Máximo valor Luminosidade: ");
  Serial.println(configs.maxLuminosidade);
  Serial.print("Estado Buzzer: ");
  Serial.println(configs.estadoBuzzer);
  Serial.print("Tipo de som: ");
  Serial.println(configs.tipoSomBuzzer);
  Serial.print("Período de medição: ");
  Serial.println(configs.periodoMedicao);
  Serial.print("Idioma: ");
  Serial.println(configs.idioma);
  Serial.print("Fuso Horário: ");
  Serial.println(configs.fusoHorario);
  Serial.print("Unidade de Temperatura: ");
  Serial.println(configs.unidadeTemperatura);

}

void mostarConfiguracoesEEPROM(){
  Configuracoes configs = resgataConfiguracoesNaEEPROM();

  Serial.print("Minimo valor LDR: ");
  Serial.println(configs.minLDR);
  Serial.print("Máximo valor LDR: ");
  Serial.println(configs.maxLDR);
  Serial.print("Minimo valor Temperatura: ");
  Serial.println(configs.minTemperatura);
  Serial.print("Máximo valor Temperatura: ");
  Serial.println(configs.maxTemperatura);
  Serial.print("Mínimo valor Umidade: ");
  Serial.println(configs.minUmidade);
  Serial.print("Máximo valor Umidade: ");
  Serial.println(configs.maxUmidade);
  Serial.print("Mínimo valor Luminosidade: ");
  Serial.println(configs.minLuminosidade);
  Serial.print("Máximo valor Luminosidade: ");
  Serial.println(configs.maxLuminosidade);
  Serial.print("Estado Buzzer: ");
  Serial.println(configs.estadoBuzzer);
  Serial.print("Tipo de som: ");
  Serial.println(configs.tipoSomBuzzer);
  Serial.print("Período de medição: ");
  Serial.println(configs.periodoMedicao);
  Serial.print("Idioma: ");
  Serial.println(configs.idioma);
  Serial.print("Fuso Horário: ");
  Serial.println(configs.fusoHorario);
  Serial.print("Unidade de Temperatura: ");
  Serial.println(configs.unidadeTemperatura);

}

void mostrarDadosNaSerial(){
  Serial.print("Temperatura: ");
  Serial.print(temperatura);
  Serial.print(" C° ");
  Serial.print(" Umidade: ");
  Serial.print(umidade);
  Serial.print(" % ");
  Serial.print(" Luminosidade: ");
  Serial.print(luminosidade);
  Serial.print(" % ");
  Serial.print(" Horario: ");
  Serial.println(montarStringTimeStamp(timestampAtual));
}

void mostrarDadosFalhaSalvos(){
  int enderecoAtual = ENDERECO_INICIAL_REGISTROS_NA_EEPROM;
  int ultimoEnderecoLivre = EEPROM.read(0);

  while(enderecoAtual < ultimoEnderecoLivre){

    DadosFalha dadosResgatados = resgataDadosFalhaEEPROMPorEndereco(enderecoAtual);
    
    Serial.print("Data e Hora: ");
    Serial.print(dadosResgatados.dataHora.timestamp());
    Serial.print(" Erro: ");
    Serial.println(dadosResgatados.erro);

    enderecoAtual += 6;
  }
}

DadosFalha resgataDadosFalhaEEPROMPorIndice(int indice){
  int enderecoCorrespondente = encontraEnderecoPorIndice(indice);
  if(enderecoCorrespondente < 0){
    DadosFalha dado;
    dado.erro = 0;
    return dado; //retorna um DadosFalha com erro 0 e data fora do comum;
  }
  return resgataDadosFalhaEEPROMPorEndereco(enderecoCorrespondente);
}


DadosFalha resgataDadosFalhaEEPROMPorEndereco(int endereco){
  
  DadosFalha dadosResgatados;
  dadosResgatados.dataHora = DateTime(eeprom_read_dword(endereco));
  dadosResgatados.erro = eeprom_read_word(endereco + 4);

  return dadosResgatados;
}

int encontraEnderecoPorIndice(int indice){
  int endereco = ENDERECO_INICIAL_REGISTROS_NA_EEPROM + (indice*6);
  if(endereco > 1023 || endereco < ENDERECO_INICIAL_REGISTROS_NA_EEPROM || endereco >= EEPROM.read(0))
    return -1;
  return endereco;
}

Configuracoes resgataConfiguracoesNaEEPROM(){
  Configuracoes configs;
  int enderecoAtual = ENDERECO_INICIAL_CONFIGURACOES;
  configs.minLDR = eeprom_read_word(enderecoAtual);
  enderecoAtual += 2;
  configs.maxLDR = eeprom_read_word(enderecoAtual);
  enderecoAtual += 2;
  configs.minTemperatura = EEPROM.read(enderecoAtual);
  enderecoAtual ++;
  configs.maxTemperatura = EEPROM.read(enderecoAtual);
  enderecoAtual ++;
  configs.minUmidade = EEPROM.read(enderecoAtual);
  enderecoAtual ++;
  configs.maxUmidade = EEPROM.read(enderecoAtual);
  enderecoAtual ++;
  configs.minLuminosidade = EEPROM.read(enderecoAtual);
  enderecoAtual ++;
  configs.maxLuminosidade = EEPROM.read(enderecoAtual);
  enderecoAtual ++;
  configs.estadoBuzzer = EEPROM.read(enderecoAtual);
  enderecoAtual ++;
  configs.tipoSomBuzzer = EEPROM.read(enderecoAtual);
  enderecoAtual ++;
  configs.periodoMedicao = eeprom_read_word(enderecoAtual);
  enderecoAtual += 2;
  configs.idioma = EEPROM.read(enderecoAtual);
  enderecoAtual ++;
  configs.fusoHorario = eeprom_read_word(enderecoAtual);
  enderecoAtual += 2;
  configs.unidadeTemperatura = EEPROM.read(enderecoAtual);

  return configs;
}

void gravarDadosFalhaNaEEPROM(byte codFalha, byte medida, DateTime timestamp){
  int ultimoEnderecoLivre = EEPROM.read(0);
  
  // Se no último endereço a armazenar, não houver espaço suficiente até o tamanho da memória, retorna para o começo dos registros
  if((1023 - ultimoEnderecoLivre) < 6 )
  {
    ultimoEnderecoLivre = ENDERECO_INICIAL_REGISTROS_NA_EEPROM;
    EEPROM.write(0, ENDERECO_INICIAL_REGISTROS_NA_EEPROM);
  }

  uint32_t dataHoraEmSegundos = timestamp.unixtime();
  uint16_t erro = montarCodigoErroEmUInt16(codFalha, medida);

  eeprom_write_dword(ultimoEnderecoLivre, dataHoraEmSegundos);
  ultimoEnderecoLivre += 4; // número de bytes em um double word ou uint32
  eeprom_write_word(ultimoEnderecoLivre, erro);
  ultimoEnderecoLivre += 2; // número de bytes em um word ou uint16

  EEPROM.write(0,ultimoEnderecoLivre);
}

void gravarConfiguracoesNaEEPROM(Configuracoes configs){
  int enderecoAtual = ENDERECO_INICIAL_CONFIGURACOES;
  eeprom_update_word(enderecoAtual,configs.minLDR);
  enderecoAtual += 2;
  eeprom_update_word(enderecoAtual,configs.maxLDR);
  enderecoAtual += 2;
  EEPROM.write(enderecoAtual, configs.minTemperatura);
  enderecoAtual ++;
  EEPROM.write(enderecoAtual, configs.maxTemperatura);
  enderecoAtual ++;
  EEPROM.write(enderecoAtual, configs.minUmidade);
  enderecoAtual ++;
  EEPROM.write(enderecoAtual, configs.maxUmidade);
  enderecoAtual ++;
  EEPROM.write(enderecoAtual, configs.minLuminosidade);
  enderecoAtual ++;
  EEPROM.write(enderecoAtual, configs.maxLuminosidade);
  enderecoAtual ++;
  EEPROM.write(enderecoAtual, configs.estadoBuzzer);
  enderecoAtual ++;
  EEPROM.write(enderecoAtual, configs.tipoSomBuzzer);
  enderecoAtual ++;
  eeprom_update_word(enderecoAtual,configs.periodoMedicao);
  enderecoAtual += 2;
  EEPROM.write(enderecoAtual, configs.idioma);
  enderecoAtual ++;
  eeprom_update_word(enderecoAtual,configs.fusoHorario);
  enderecoAtual += 2;
  EEPROM.write(enderecoAtual, configs.unidadeTemperatura);
}

uint16_t montarCodigoErroEmUInt16(byte codFalhaComponente, byte medida){
  uint16_t codigoErro = (codFalhaComponente*1000) + medida;
  
  return codigoErro;
}

void atualizarMedidas(){
  umidade = dht.readHumidity();
  temperatura = dht.readTemperature(); 
  luminosidade = analogRead(LDR);
  luminosidade = map(luminosidade, config.minLDR, config.maxLDR, 100, 0); // corrigir para o caso do ldr físico 
}

double converterCelsiusParaFahrenheit(double temperaturaCelsius){
  double temperaturaFahrenheit = (temperaturaCelsius * 1,8) + 32;
  return temperaturaFahrenheit;
}

double converterCelsiusParaKelvin(double temperaturaCelsius){
  double temperaturaKelvin = temperaturaCelsius + 273.15;
  return temperaturaKelvin;
}

void voltarParaConfiguracoesDeFabrica(){
  config.minLDR = LIMITE_MINIMO_LDR;
  config.maxLDR = LIMITE_MAXIMO_LDR;
  config.minTemperatura = LIMITE_MINIMO_SENSOR_TEMPERATURA_INICIAL;
  config.maxTemperatura = LIMITE_MAXIMO_SENSOR_TEMPERATURA_INICIAL;
  config.minUmidade = LIMITE_MINIMO_SENSOR_UMIDADE_INICIAL;
  config.maxUmidade = LIMITE_MAXIMO_SENSOR_UMIDADE_INICIAL;
  config.minLuminosidade = LIMITE_MINIMO_SENSOR_LUMINOSIDADE_INICIAL;
  config.maxLuminosidade = LIMITE_MAXIMO_SENSOR_LUMINOSIDADE_INICIAL;
  config.idioma = IDIOMA_INICIAL;
  config.estadoBuzzer = ESTADO_BUZZER_INICIAL;
  config.tipoSomBuzzer = TIPO_SOM_BUZZER_INICIAL;
  config.periodoMedicao = PERIODO_MEDICAO_INICIAL;
  config.fusoHorario = FUSO_HORARIO_ATUAL;
  config.unidadeTemperatura = UNIDADE_TEMPERATURA_INICIAL;

  gravarConfiguracoesNaEEPROM(config);
}

void limparRegistrosFalhaEEPROM(){
  EEPROM.write(0,ENDERECO_INICIAL_REGISTROS_NA_EEPROM); // Reinicia o último endereço livre para registro
  for (int i = ENDERECO_INICIAL_REGISTROS_NA_EEPROM; i < EEPROM.length(); i++) {
  EEPROM.write(i, 0xFF); // Apaga todos os dados
  }
}



// Métodos Lucas e Victor:
void transicaoInicial() {
  lcd.clear();

  for (int i = 0; i < 16; i++) {
    lcd.setCursor(i, 0);
    lcd.print("=");
    delay(50);
    lcd.setCursor(i, 0);
    lcd.print(" ");
  }

  for (int i = 15; i >= 0; i--) {
    lcd.setCursor(i, 1);
    lcd.print("=");
    delay(50);
    lcd.setCursor(i, 1);
    lcd.print(" ");
  }

  for (int i = 0; i < 2; i++) {
    lcd.setCursor(0, i);
    lcd.print("=");
    lcd.setCursor(15, i);
    lcd.print("=");
    delay(50);
    lcd.setCursor(0, i);
    lcd.print(" ");
    lcd.setCursor(15, i);
    lcd.print(" ");
  }

  lcd.clear();
}

void imprimirIcone(){
  
  lcd.setCursor(12, 0);
  lcd.write(3);
  lcd.setCursor(12, 1);
  lcd.write(2);
  lcd.setCursor(13, 0);
  lcd.write(1);
  lcd.setCursor(13, 1);
  lcd.write(0);
  lcd.setCursor(14, 0);
  lcd.write(5);
  lcd.setCursor(14, 1);
  lcd.write(4);
}

void inicializaMenu() {
  lcd.createChar(0, iconeWeathino1);
  lcd.createChar(1, iconeWeathino2);
  lcd.createChar(2, iconeWeathino3);
  lcd.createChar(3, iconeWeathino4);
  lcd.createChar(4, iconeWeathino5);
  lcd.createChar(5, iconeWeathino6);
  
  transicaoInicial();
  
  lcd.clear();
  lcd.print("Weathuino");
  lcd.setCursor(0, 1);
  lcd.print("=========");
  imprimirIcone();
  
  delay(3000);

  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Iniciando");

  for (int i = 0; i < 3; i++) {
    lcd.print(".");
    delay(1000);
  }

  lcd.clear();
}

void exibeTextoNoLCD(String primeiraLinha, String segundaLinha){
    lcd.clear();
    lcd.print(primeiraLinha);
    lcd.setCursor(0,1);
    lcd.print(segundaLinha);
}