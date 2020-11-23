# SimpleExchangeRatesAPI
Simple currency exchange rates API for data taken from ECB (European Central Bank)

STEPS TO TEST THE PROJECT:

Prerequisites:
1) Install supplied Redis-x64-3.2.100.msi
  1.1) (Optionally) instal RDM or other Redis db management GUI
3) Ensure to run Redis server
4) Install Java JDK 8u271-windows-x64 and set JAVA OPTS Environment Variable to enable testing with Gatling tool
5) Download Gatling 

VISUAL STUDIO:
1) Publish database
2) Run
3) Test requests with Swagger

Gatling Load Testing:
1) Move "RecordedSimulation.scala" to Gatling folder > user-files > simulations > 
2) open console and go to Gatling folder > bin >
3) run gatling.bat
4) choose simulation
