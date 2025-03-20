// bibliotecas importadas
#include <avr/eeprom.h>
#include <EEPROM.h>
#include <LiquidCrystal_I2C.h>
#include <DHT.h>
#include <RTClib.h>
#include <Wire.h>
#include <Arduino.h>

#define DHTTYPE DHT11 // No projeto físico trocar para DHT11
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
#define LIMITE_MAXIMO_SENSOR_TEMPERATURA_INICIAL 22
#define LIMITE_MINIMO_SENSOR_UMIDADE_INICIAL 30
#define LIMITE_MAXIMO_SENSOR_UMIDADE_INICIAL 75
#define LIMITE_MINIMO_SENSOR_LUMINOSIDADE_INICIAL 0
#define LIMITE_MAXIMO_SENSOR_LUMINOSIDADE_INICIAL 50
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

//Configurações do sistema
Configuracoes resgataConfiguracoesNaEEPROM();
Configuracoes config = resgataConfiguracoesNaEEPROM();

// Bytes do ícone na inicialização
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

enum TELAS {
  MEDICOES,
  TIMESTAMP,
  ALERTA
};
TELAS TELA_ATUAL = MEDICOES;

void setup() {
  // Reset dos dados apenas para título de teste
  limparRegistrosFalhaEEPROM();
  voltarParaConfiguracoesDeFabrica();

  // Inicialização do serial
  Serial.begin(9600);

  // Inicialização dos pinos
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

  // ajusta o rtc para o horário atual da gravação
  rtc.adjust(DateTime(F(__DATE__), F(__TIME__)));
  inicializaMenu();
}

void loop() {
  // Obtendo o timestamp atual de acordo com o GMT configurado
  timestampAtual = rtc.now();

  atualizarMedidas();
  // mostraDadosNaSerial();

  // Validação dos valores obtidos por meio dos sensores
  bool valoresDosSensoresValidos = !isnan(temperatura) && !isnan(umidade);
  if (valoresDosSensoresValidos)
  {
    desligaBuzzer();
  } 
  else{
    gravarDadosFalhaNaEEPROM(COD_ERRO_SENSOR_DHT_FALHA, 0, timestampAtual);
    acionaBuzzer();
  }

  // atualiza a verificação dos estados das medidas
  bool temperaturaIrregular = temperaturaForaLimite(temperatura);
  bool umidadeIrregular = umidadeForaLimite(umidade);
  bool luminosidadeIrregular = luminosidadeForaLimite(luminosidade);
  bool ambienteIrregular = temperaturaIrregular || umidadeIrregular || luminosidadeIrregular;
  
  // grava e notifica o erro caso algum dado esteja fora do padrão
  if(ambienteIrregular) {
    String mensagemDeAlerta = "";

    if(temperaturaIrregular) {
      gravarDadosFalhaNaEEPROM(COD_ERRO_SENSOR_TEMPERATURA_MEDIDA, temperatura, timestampAtual);
      mensagemDeAlerta = "Temp.: " + String(temperatura) + "C";
    }
    if(umidadeIrregular) {
      gravarDadosFalhaNaEEPROM(COD_ERRO_SENSOR_UMIDADE_MEDIDA, umidade, timestampAtual);
      mensagemDeAlerta = "Umid.: " + String(umidade) + "%";
    }
    if(luminosidadeIrregular) {
      gravarDadosFalhaNaEEPROM(COD_ERRO_SENSOR_LUMINOSIDADE_MEDIDA, luminosidade, timestampAtual);
      mensagemDeAlerta = "Lumin.: " + String(luminosidade) + "%";
    }

    TELA_ATUAL = TELAS::ALERTA;
    exibeTextoNoLCD("AVISO!!!", mensagemDeAlerta);

    acionaSinalVermelho();
    acionaBuzzer();
  }
  else {
    acionaSinalVerde();
    desligaBuzzer();
  }

  // gerenciamento das telas em exibição
  switch(TELA_ATUAL) {
    case TELAS::MEDICOES:
      gerenciaTelaMedicoes();
      delay(1000);
      break;
    case TELAS::TIMESTAMP:
      gerenciaTelaTimestamp();
      delay(1000);
      break;
    case TELAS::ALERTA:
      if (!ambienteIrregular) TELA_ATUAL = TELAS::MEDICOES;
      delay(5000);
      break;
  }

  bool exportacaoSolicitada = digitalRead(BOTAO_M);
  if (exportacaoSolicitada) {
    exportaRegistrosViaSerial();
  }
}

// controla a tela de exibir as medidas
void gerenciaTelaMedicoes() {
  bool botaoDireitoPressionado = digitalRead(BOTAO_D);
  if (botaoDireitoPressionado) {
    TELA_ATUAL = TELAS::TIMESTAMP;
  } else {
    String dados = padRight(" " + String(temperatura) + "C", 4) + "  " +
    padLeft(String(umidade) + "%", 4) + " " +
    padLeft(String(luminosidade) + "%", 4);
    exibeTextoNoLCD(" TEMP UMID LUMI", dados);    
  }
}

// utilitário para preencher uma string à esquerda
String padLeft(String texto, int tamanho) {
  String novaString = texto;
  while (novaString.length() < tamanho)
    novaString = " " + novaString;
  return novaString;
}

// utilitário para preencher uma string à direita
String padRight(String texto, int tamanho) {
  String novaString = texto;
  while (novaString.length() < tamanho)
    novaString = novaString + " ";
  return novaString;
}

// controla a tela de exibir timestamp
void gerenciaTelaTimestamp() {
  bool botaoEsquerdoPressionado = digitalRead(BOTAO_E);
  if (botaoEsquerdoPressionado) {
    TELA_ATUAL = TELAS::MEDICOES;
  } else {
    String data = montaStringData(timestampAtual);
    String hora = montaStringHora(timestampAtual);
    exibeTextoNoLCD("   " + data, "    " + hora);
  }
}

bool temperaturaForaLimite(float temperaturaMedida){
  return temperaturaMedida < config.minTemperatura || temperaturaMedida > config.maxTemperatura;
}

bool umidadeForaLimite(unsigned int umidadeMedida){
  return umidadeMedida < config.minUmidade || umidadeMedida > config.maxUmidade;
}

bool luminosidadeForaLimite(unsigned int luminosidadeMedida){
  return luminosidadeMedida < config.minLuminosidade || luminosidadeMedida > config.maxLuminosidade;
}

// monta string do timestamp para uso em alguma saída visual
String montarStringTimeStamp(DateTime dataHora){
  return montaStringData(dataHora) + " " + montaStringHora(dataHora);
}

// monta uma data por meio de um timestamp
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

// monta uma hora por meio de um timestamp
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

// aciona o buzzer em som contínuo
void acionaBuzzer(){
  tone(BUZZER, 600);
}

// desliga o buzzer
void desligaBuzzer(){
  noTone(BUZZER); 
}

void acionaSinalVermelho(){
    analogWrite(LED_VERMELHO, 255);
    analogWrite(LED_VERDE, 0);
    analogWrite(LED_AZUL, 0);
}

void acionaSinalVerde(){
    analogWrite(LED_VERMELHO, 0);
    analogWrite(LED_VERDE, 255);
    analogWrite(LED_AZUL, 0);
}

// método auxiliar apenas para exibir dados na serial
void mostraDadosNaSerial(){
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

// busca e exibe os registros de anormalidade salvos na EEPROM
void mostraDadosFalhaSalvos(){
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

// obtém um registro de anormalidade por meio de seu endereço na eeprom
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

// Obtém as configurações de fábrica da placa weathuino
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

// grava as configurações do Weathuino na EEPROM
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

// atualiza as variáveis de temperatura, umidade e luminosidade
void atualizarMedidas(){
  umidade = dht.readHumidity();
  temperatura = dht.readTemperature(); 
  luminosidade = analogRead(LDR);
  // luminosidade = map(luminosidade, config.minLDR, config.maxLDR, 100, 0); // usar no simulador 
  luminosidade = map(luminosidade, config.minLDR, config.maxLDR, 0, 100); // usar no fisico
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

//deixa todos os bytes da memória EEPROM em 0 e coloca o ultimo endereço livre no inicial
void limparRegistrosFalhaEEPROM(){
  EEPROM.write(0,ENDERECO_INICIAL_REGISTROS_NA_EEPROM); // Reinicia o último endereço livre para registro
  for (int i = ENDERECO_INICIAL_REGISTROS_NA_EEPROM; i < EEPROM.length(); i++) {
  EEPROM.write(i, 0xFF); // Apaga todos os dados
  }
}

// Exibe a transição exibida na inicialização do LCD
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

// Método para Exibir o ícone da Aplicação
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

// Método para exibir o ícone da aplicação
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

// Método para exibir textos na primeira e segunda linha do display
void exibeTextoNoLCD(String primeiraLinha, String segundaLinha){
    lcd.clear();
    lcd.print(primeiraLinha);
    lcd.setCursor(0,1);
    lcd.print(segundaLinha);
}

// método para formatar o json exibido via serial com os registros da eeprom
void exportaRegistrosViaSerial() {
  int enderecoAtual = ENDERECO_INICIAL_REGISTROS_NA_EEPROM;
  int ultimoEnderecoLivre = EEPROM.read(0);

  Serial.println("[");
  while(enderecoAtual < ultimoEnderecoLivre){
    DadosFalha dadosResgatados = resgataDadosFalhaEEPROMPorEndereco(enderecoAtual);
    int codigoErro = static_cast<int>(dadosResgatados.erro/1000);
    int valorMedida = dadosResgatados.erro - static_cast<int>(dadosResgatados.erro/1000)*1000;
    String sensorMedida="";

    switch(codigoErro){
      case 1:
        sensorMedida = "temperatura";
      break;
      case 2:
        sensorMedida = "umidade";
      break;
      case 3:
        sensorMedida = "luminosidade";
      break;
      case 4:
        sensorMedida = "ErroSensorDHT";
      break;
    }

    String registroJson = "";
    registroJson = String(registroJson + "{\"timestamp\":" + "\"" + String(dadosResgatados.dataHora.timestamp()) + "\","+
    "\"" + sensorMedida + "\":" + "\"" +String(valorMedida)+ "\"" + "}");

    enderecoAtual += 6;
    if(enderecoAtual < ultimoEnderecoLivre)
      registroJson = String(registroJson + ",");
    Serial.println(registroJson);
  }
  Serial.println("]");
}
