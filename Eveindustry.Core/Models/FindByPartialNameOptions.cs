namespace Eveindustry.Core.Models
{
    /// <summary>
    /// Search configuration options.
    /// </summary>
    public enum FindByPartialNameOptions
    {
        /// <summary>
        /// Search configuration to look for strings starting with provided sample. 
        /// </summary>
        StartingWith,
        /// <summary>
        ///Search configuration to look for strings starting with provided sample.
        /// </summary>
        Contains,
        ///Search configuration to look for strings exactly same as provided sample (ignoring case).
        ExactMatch
    }
}