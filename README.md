# SimpleExchangeRatesAPI :moneybag: :rocket:
## What is this?

A simple, efficient currency exchange rates API for data taken from ECB (European Central Bank) v.1(*BETA*)

## How to use it?
For now - (it's a BETA version) you can run this project locally on your computer. Just clone this repo and follow the steps below to test it. You can compose path request more easily than on ECB API. Below you'll find example endpoint and info on path composition.



**Example:**

![](https://raw.githubusercontent.com/tomazi776/simpleexchangeratesapi/main/images/example.png)

![](https://raw.githubusercontent.com/tomazi776/simpleexchangeratesapi/main/images/JPY_USD_PLN.png)

Data is coming in no particular order, although sorting by currency code is planned for next version.

## Structure: :statue_of_liberty:

Endpoint structure is fairly simple and straightforward, but for point of reference - endpoint composition is shown below.

* The `single` part refers to the singular observation, meaning we  want to get data for a specific day. Takes in a boolean `[true/false]` (* true -> accounts only for startDate, ignores endDate, false -> accounts for both *)
* By default if we don't pass end date while specyfying single -> false, we'll receive data from start date until now(newest). 
* Multiple `currencyCodes` can only be separated by "+"

**Endpoint composition:**

*localhost* /
![](https://raw.githubusercontent.com/tomazi776/simpleexchangeratesapi/main/images/Endpoint.png)

***

## STEPS TO TEST THE PROJECT:

Prerequisites:
1) Install supplied Redis-x64-3.2.100.msi
2) (Optionally) instal RDM or other Redis db management GUI to see cached data more easily
3) Ensure to run Redis server
4) Install Java **JDK 8u271-windows-x64** and set `JAVA_TOOL_OPTIONS` environment variable to `-Dfile.encoding=UTF8` to be able to use Gatling for Load testing
5) Download [Gatling](https://gatling.io/open-source/start-testing/)

**VISUAL STUDIO:**
1) Publish database
2) Run
3) Test requests with Swagger


**Gatling Load Testing:**
1) Move "RecordedSimulation.scala" to Gatling folder > user-files > simulations > 
2) open console and go to Gatling folder > bin >
3) run gatling.bat
4) choose simulation

`*`Remember to add ApiKey to request headers when testing or comment the Authorization attribute on the controller.