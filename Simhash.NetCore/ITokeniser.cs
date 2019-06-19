using System.Collections.Generic;

namespace Simhash.NetCore
{
    /// <summary>
    /// 分词接口
    /// </summary>
    public interface ITokeniser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        IEnumerable<string> Analytical(string input);
    }
}