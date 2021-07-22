using System;
using System.Drawing;

namespace _3D_Reconstruction
{
    public class PositionObj
    {
        /// <summary> Метод вычисления моментов и определения улга поворота объекта по плоскому изображению</summary>
        /// <param name="bitmap"></param>
        /// <returns> Модель с результами расчета</returns>
        public InfoObj CalculateInfo(Bitmap bitmap)
        {
            Accord.Imaging.Moments.RawMoments moment = new Accord.Imaging.Moments.RawMoments(bitmap);
            Accord.Imaging.Moments.CentralMoments centralMoments = new Accord.Imaging.Moments.CentralMoments(moment);
           //Костыль для позиционирования объекта в горизонтальном положении 
            var angle = centralMoments.GetOrientation() * (180) / Math.PI;
            if (angle > 90)
                angle = 180 - angle;

            InfoObj infoObj = new InfoObj();
            infoObj.Angle = angle;
            infoObj.CenterX = moment.CenterX;
            infoObj.CenterY = moment.CenterY;
            infoObj.M11 = moment.M11;
            infoObj.M20 = moment.M20;
            infoObj.M02 = moment.M02;
            return infoObj;
        }
    }
}
