# AR-Brick-Firmware

- Platinen sind auf der nas unter "intern\Projekte\QFabLab\AR-Quantenkoffer\Platinen"
- STLs sind auch auf der nas unter "intern\Projekte\QFabLab\AR-Quantenkoffer\stls"

# Struktur

- lib/encoder (library für den Rotary Encoder)
- lib/pcf8574AN (library für den GPIO-Expander)
- src/MenuStructure (GUI-Menü)

# Funktionsweise

- Nachdem ein Spielstein eingesteckt worden ist muss gewartet werden, bis dieser hochgefahren ist und auf dem Display seinen Typen anzeigt
- Das ist, weil der Pi die I2C-Adressen auf dem bus abfragt und sobald eine neue erscheint ihre Position abspeichert
- Steckt man zwei spielsteine direkt hintereinander ein ohne auf das hochfahren zu warten kann die Position nicht mehr über die Matrix zugeordnet wertden.

## Spielfeld 
- das Spielfeld stellt zum eine Matrix bereit über die via GPIO die Position des Spielsteins abgefragt werden kann
- Außerdem bietet das Spielfeld verbindungen zu I2C, eine 2bit-Kodierung für die Rotation und Versorgungsspannugn 

## Spielstein
- Der Spielstein verbindet sich mit den entsprechenden pins, und liest seine Rotation
- Der Spielstein fungiert als I2C-Slave um werte wie den eigenen Typen, die Rotation und die Feineinstellung zurückgeben zu können
- Außerdem verfügt er über einen Software I2C-Bus um mit dem GPIO-Expander zu kommunizieren
- Menü
  - Das Menü kann über die Rotation des RotaryEncoders den Spielsteintypen umstellen und über Knopfdruck in die "Feineinstellung" des jeweiligen typen gehen. 
  - Ein zweiter Knopfdruck geht wieder zurück in die "Typauswahl" 
  - Feineinstellungen gibt es aktuell nur beim 90° spiegel

# BOM

- Basisplatine
- PCF8574AN - GPIO-Expander [Reichelt](https://www.reichelt.de/remote-8-bit-i-o-expander-for-i2c-bus-pdip-16-pcf-8574-an-p216403.html?PROVID=2788&gad_source=1&gclid=CjwKCAiA9IC6BhA3EiwAsbltODcHnrU-YTUJUlkGrAGHwNF8oVP8MXblSkJ48YKtkgBF-pSqnhMfTxoCNYsQAvD_BwE)
- DEBO LCD 1.28" - Display [Reichelt](https://www.reichelt.de/entwicklerboards-display-lcd-1-28-rund-240-x-240-pixel-debo-lcd-1-28-p334929.html?PROVID=2788&gad_source=1&gclid=CjwKCAiA9IC6BhA3EiwAsbltOGF0dKsVbQqsdGkb_67O7-t5j7W8yEjmnckzFzGIsHfNyDFkiI4EuRoCAjMQAvD_BwE)
- 3x 10k widerstand
- XIAO SAMD21 - Mikrocontroller [Reichelt](https://www.reichelt.de/xiao-samd21-samd21-cortex-m0-32-bit-arm-xiao-samd21-p350827.html?&trstct=pos_1&nbc=1)
- Rotary Encoder Board KY-40 [Reichelt](https://www.reichelt.de/entwicklerboards-drehwinkel-encoder-ky-040-debo-encoder-p282545.html?PROVID=2788&gad_source=1&gclid=CjwKCAiA9IC6BhA3EiwAsbltOI7A1OxBf4NIrF5Tj8O7oO5_Ad_qazDERAcE-ugRYLz68fnlVH93sRoC3BgQAvD_BwE)
- Federkontakte [AmazonLink](https://www.amazon.de/dp/B07FPCPX8X?ref=ppx_yo2ov_dt_b_fed_asin_title) / [Dimensions](./images/pogo_pin.png)
