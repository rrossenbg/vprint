/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;

namespace PremierTaxFree.Controls
{
    public interface IDirtyControl
    {
        /// <summary>
        /// Control is dirty if it has been changed
        /// </summary>
        bool IsDirty { get; set; }
    }

    public interface ISettingsControl : IDirtyControl
    {
        /// <summary>
        /// Reads the initial state of the control
        /// </summary>
        void Read();
        /// <summary>
        /// Validate data here
        /// </summary>
        bool Verify();
        /// <summary>
        /// Saves the state of the control
        /// </summary>
        void Save();
        /// <summary>
        /// Updates any environment that 
        /// may be affected by the changes
        /// </summary>
        void UpdateEnvironment();
    }
}
