using System.Collections.Generic;
using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.Core.Sde.StaticDataModels
{
    /// <summary>
    /// Eve Type info DTO for SDE.
    /// </summary>
    [MessagePackObject]
    internal sealed class SdeEveBasicType
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((SdeEveBasicType) obj);
        }

        /// <summary>
        /// Equality check based on Id property
        /// </summary>
        /// <param name="other">other object</param>
        /// <returns>true if objects are equals. false otherwise. </returns>
        private bool Equals(SdeEveBasicType other)
        {
            return this.Id == other.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (int)this.Id;
        }

        /// <summary>
        /// Gets or sets eve type id.
        /// </summary>
        [IgnoreMember]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets GroupID.
        /// </summary>
        [YamlMember(Alias = "groupID")]
        [Key(0)]
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets Mass.
        /// </summary>
        [YamlMember(Alias = "mass")]
        [Key(1)]
        public double Mass { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [YamlMember(Alias = "name")]
        [Key(2)]
        public LocalizedText Name { get; set; }

        /// <summary>
        /// Gets or sets PortionSize.
        /// </summary>
        [YamlMember(Alias = "portionSize")]
        [Key(3)]
        public double PortionSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product is published.
        /// </summary>
        [YamlMember(Alias = "published")]
        [Key(4)]
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets volume.
        /// </summary>
        [YamlMember(Alias = "volume")]
        [Key(5)]
        public double Volume { get; set; }

        /// <summary>
        /// Gets or sets radius.
        /// </summary>
        [YamlMember(Alias = "radius")]
        [Key(6)]
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        [YamlMember(Alias = "description")]
        [Key(7)]
        public LocalizedText Description { get; set; }

        /// <summary>
        /// Gets or sets GraphicID.
        /// </summary>
        [YamlMember(Alias = "graphicID")]
        [Key(8)]
        public int GraphicId { get; set; }

        /// <summary>
        /// Gets or sets SoundID.
        /// </summary>
        [YamlMember(Alias = "soundID")]
        [Key(9)]
        public int SoundId { get; set; }

        /// <summary>
        /// Gets or sets IconId.
        /// </summary>
        [YamlMember(Alias = "iconID")]
        [Key(10)]
        public int IconId { get; set; }

        /// <summary>
        /// Gets or sets RaceID.
        /// </summary>
        [YamlMember(Alias = "raceID")]
        [Key(11)]
        public int RaceId { get; set; }

        /// <summary>
        /// Gets or sets SofFactionName.
        /// </summary>
        [YamlMember(Alias = "sofFactionName")]
        [Key(12)]
        public string SofFactionName { get; set; }

        /// <summary>
        /// Gets or sets BasePrice.
        /// </summary>
        [YamlMember(Alias = "basePrice")]
        [Key(13)]
        public double BasePrice { get; set; }

        /// <summary>
        /// Gets or sets MarketGroupID.
        /// </summary>
        [YamlMember(Alias = "marketGroupID")]
        [Key(14)]
        public int MarketGroupID { get; set; }

        /// <summary>
        /// Gets or sets Capacity.
        /// </summary>
        [YamlMember(Alias = "capacity")]
        [Key(15)]
        public double Capacity { get; set; }

        /// <summary>
        /// Gets or sets MetaGroupID.
        /// </summary>
        [YamlMember(Alias = "metaGroupID")]
        [Key(16)]
        public int MetaGroupId { get; set; }

        /// <summary>
        /// Gets or sets VariationParentTypeId.
        /// </summary>
        [YamlMember(Alias = "variationParentTypeID")]
        [Key(17)]
        public int VariationParentTypeId { get; set; }

        /// <summary>
        /// Gets or sets FactionId.
        /// </summary>
        [YamlMember(Alias = "factionID")]
        [Key(18)]
        public int FactionId { get; set; }

        /// <summary>
        /// Gets or sets masteries.
        /// </summary>
        [YamlMember(Alias = "masteries")]
        [Key(19)]
        public Dictionary<int, int[]> Masteries { get; set; }

        /// <summary>
        /// Gets or sets SofMaterialSetId.
        /// </summary>
        [YamlMember(Alias = "sofMaterialSetID")]
        [Key(20)]
        public string SofMaterialSetId { get; set; }

        /// <summary>
        /// Gets or sets Traits.
        /// </summary>
        [YamlMember(Alias = "traits")]
        [Key(21)]
        public EveTypeTraits Traits { get; set; }
    }
}