using System.Collections.Generic;
using MessagePack;
using YamlDotNet.Serialization;

namespace Eveindustry.StaticDataModels
{
    /// <summary>
    /// Eve Type info DTO for SDE.
    /// </summary>
    [MessagePackObject]
    public class EveType
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

            return this.Equals((EveType) obj);
        }

        protected bool Equals(EveType other)
        {
            return this.Id == other.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (int)this.Id;
        }

        /// <summary>
        /// Gets or sets item price (data is not included in eve SDE)
        /// </summary>
        [IgnoreMember]
        public decimal PriceSell { get; set; }

        /// <summary>
        /// Gets or sets item price (data is not included in eve SDE)
        /// </summary>
        [IgnoreMember]
        public decimal PriceBuy { get; set; }

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
        public virtual int GroupId { get; set; }

        /// <summary>
        /// Gets or sets Mass.
        /// </summary>
        [YamlMember(Alias = "mass")]
        [Key(1)]
        public virtual double Mass { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [YamlMember(Alias = "name")]
        [Key(2)]
        public virtual LocalizedText Name { get; set; }

        /// <summary>
        /// Gets or sets PortionSize.
        /// </summary>
        [YamlMember(Alias = "portionSize")]
        [Key(3)]
        public virtual double PortionSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product is published.
        /// </summary>
        [YamlMember(Alias = "published")]
        [Key(4)]
        public virtual bool Published { get; set; }

        /// <summary>
        /// Gets or sets volume.
        /// </summary>
        [YamlMember(Alias = "volume")]
        [Key(5)]
        public virtual double Volume { get; set; }

        /// <summary>
        /// Gets or sets radius.
        /// </summary>
        [YamlMember(Alias = "radius")]
        [Key(6)]
        public virtual double Radius { get; set; }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        [YamlMember(Alias = "description")]
        [Key(7)]
        public virtual LocalizedText Description { get; set; }

        /// <summary>
        /// Gets or sets GraphicID.
        /// </summary>
        [YamlMember(Alias = "graphicID")]
        [Key(8)]
        public virtual int GraphicId { get; set; }

        /// <summary>
        /// Gets or sets SoundID.
        /// </summary>
        [YamlMember(Alias = "soundID")]
        [Key(9)]
        public virtual int SoundId { get; set; }

        /// <summary>
        /// Gets or sets IconId.
        /// </summary>
        [YamlMember(Alias = "iconID")]
        [Key(10)]
        public virtual int IconId { get; set; }

        /// <summary>
        /// Gets or sets RaceID.
        /// </summary>
        [YamlMember(Alias = "raceID")]
        [Key(11)]
        public virtual int RaceId { get; set; }

        /// <summary>
        /// Gets or sets SofFactionName.
        /// </summary>
        [YamlMember(Alias = "sofFactionName")]
        [Key(12)]
        public virtual string SofFactionName { get; set; }

        /// <summary>
        /// Gets or sets BasePrice.
        /// </summary>
        [YamlMember(Alias = "basePrice")]
        [Key(13)]
        public virtual double BasePrice { get; set; }

        /// <summary>
        /// Gets or sets MarketGroupID.
        /// </summary>
        [YamlMember(Alias = "marketGroupID")]
        [Key(14)]
        public virtual int MarketGroupID { get; set; }

        /// <summary>
        /// Gets or sets Capacity.
        /// </summary>
        [YamlMember(Alias = "capacity")]
        [Key(15)]
        public virtual double Capacity { get; set; }

        /// <summary>
        /// Gets or sets MetaGroupID.
        /// </summary>
        [YamlMember(Alias = "metaGroupID")]
        [Key(16)]
        public virtual int MetaGroupId { get; set; }

        /// <summary>
        /// Gets or sets VariationParentTypeId.
        /// </summary>
        [YamlMember(Alias = "variationParentTypeID")]
        [Key(17)]
        public virtual int VariationParentTypeId { get; set; }

        /// <summary>
        /// Gets or sets FactionId.
        /// </summary>
        [YamlMember(Alias = "factionID")]
        [Key(18)]
        public virtual int FactionId { get; set; }

        /// <summary>
        /// Gets or sets masteries.
        /// </summary>
        [YamlMember(Alias = "masteries")]
        [Key(19)]
        public virtual Dictionary<int, int[]> Masteries { get; set; }

        /// <summary>
        /// Gets or sets SofMaterialSetId.
        /// </summary>
        [YamlMember(Alias = "sofMaterialSetID")]
        [Key(20)]
        public virtual string SofMaterialSetId { get; set; }

        /// <summary>
        /// Gets or sets Traits.
        /// </summary>
        [YamlMember(Alias = "traits")]
        [Key(21)]
        public virtual EveTypeTraits Traits { get; set; }
    }
}