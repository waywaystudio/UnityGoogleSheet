﻿namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public interface IType 
    {
        object DefaultValue { get; }
        object Read(string value);

        /// <summary>
        /// This is only implemented if necessary
        /// </summary> 
        string Write(object value);
    } 
}
