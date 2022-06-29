using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gtLibrary
{
    /// <summary>
    ///  기능함수의 모음이기 때문에 선언없이 바로 사용할 수 있도록 Static 선언을 한다.
    /// </summary>
    static class CSmallUtil
    {
        /// <summary>
        /// 거리의 경우 0 임에도 불구하고 자리수 오차때문에 작은 값이 발생할 수 있다.
        /// 따라서 액추에이터 설계에서 -0.1um 과 0.1 um 사이의 값은 영으로 처리할 필요가 있다.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool isZeroPosition(double data)
        {
            if (Math.Abs(data) < 1e-7)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 동일한 위치 임에도 자리수 오차때문에 다른 값이 발생할 수 있다.
        /// 따라서 액추에이터 설계에서 -0.1um 과 0.1 um 사이의 떨어진값은 같은 위치로 판단한다.
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>
        public static bool isSamePosition(double data1, double data2)
        {
            if (Math.Abs(data1-data2) < 1e-7)
                return true;
            else
                return false;
        }
    }
}
