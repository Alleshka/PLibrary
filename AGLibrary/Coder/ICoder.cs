using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AGLibrary.Coder
{
    public interface ICoder
    {
        String Code(float num); // Кодирование числа 
        float DeCode(String num); // Декодирование числа
    }
}