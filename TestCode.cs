
private decimal prevADX;
private decimal pprevADX;
private decimal prevSTR;
private decimal prevsMDI;
private decimal prevsPDI;


var VMA = candle.Close;
var MA = candle.Close;
var STR =  = candle.High - candle.Low;
var Hi = candle.High;
var Hi1 = p1C.High;
var Lo = candle.Low;
var Lo1 = p1C.Low;
var Close1 = p1C.Close;

var Bulls1 = 0.5 * (Math.Abs(Hi-Hi1)+(Hi-Hi1));
var Bears1 = 0.5 * (Math.Abs(Lo1-Lo)+(Lo1-Lo));
var Bears = Bulls1 > Bears1 ? 0 : (Bulls1 == Bears1 ? 0 : Bears1);
var Bulls = Bulls1 < Bears1 ? 0 : (Bulls1 == Bears1 ? 0 : Bulls1);

var sPDI = (10.0 * prevsPDI + Bulls) / (10.0+1);
var sMDI = (10.0 * prevsMDI + Bears) / (10.0+1);

var TR = Math.Max(Hi - Lo, Hi - Close1);
var STR = (10.0 * prevSTR + TR) / (10.0 + 1);

var PDI = STR > 0 ? sPDI/STR : 0;
var MDI = STR > 0 ? sMDI/STR: 0;
DX = (PDI + MDI) > 0 ? Math.Abs(PDI - MDI)/(PDI + MDI) : 0; 
var ADX = (10.0 * prevADX + DX) / (10.0+1);
var vADX = ADX;

var adxlow = pprevADX < prevADX? pprevADX : prevADX;
var adxmax = pprevADX > prevADX? pprevADX : prevADX;
var ADXmin = Math.Min(1000000.0, adxlow);
var ADXmax = Math.Max(-1.0, adxmax);
var Diff = ADXmax - ADXmin;
var Const = Diff > 0 ? (vADX- ADXmin) / Diff : 0;
var VarMA = ((2 - Const) * p1C.Close + Const * close) / 2;

var FanVMA = sma(VarMA,6)

pprevADX = prevADX;
prevADX = ADX;
prevSTR = STR;
prevsMDI = sMDI;
prevsPDI = sPDI;
