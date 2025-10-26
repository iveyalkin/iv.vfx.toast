// void adjustUVLenght_half(in float2 customData, in half2 uv, out half2 adjustedUv)
// {
    // uint toastLength = asuint(customData.y) & 0xFFu;

    // корректируем uv чтобы вместить все символы сообщения, .25 - это размер одного символа в атласе
    // adjustedUv = uv.xy * half2(toastLength * .25, .25);
// }

void adjustUVLenght_half(in uint toastLength, in half2 uv, out half2 adjustedUv)
{
    adjustedUv = uv.xy * half2(toastLength * .25, .25);
}

void getUV_half(in half2 uv, in float2 customData, out half2 charUV)
{
    uint charsPerFloat = 8u;

    // индекс символа в сообщении
    uint charInd = floor(uv.x * 4); // 4 _Cols

    // индекс координаты вектора, содержащий этот элемент
    uint dataInd = charInd / charsPerFloat;

    // получаем значение всех разрядов упакованных в нужный float
    uint packedData = asuint(customData[dataInd]);

    // непосредственно распаковка float и получение координат символа - строки и столбцы в атласе
    uint posInFloat = charInd % charsPerFloat;
    uint shift = (charsPerFloat - 1 - posInFloat) * 4;
    packedData = packedData >> shift;

    // коорединаты символа в атласе, координата 2bit
    uint y = packedData & 0x3;
    uint x = (packedData >> 2) & 0x3;

    float cols = .25;
    float rows = .25;

    charUV = uv;

    // корректируем uv чтобы попасть в нужный тексель атласа
    charUV.x = frac(uv.x * 4.0) * cols + x * cols; // 4 _Cols
    charUV.y += y * rows;
}