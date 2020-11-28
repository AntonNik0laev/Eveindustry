﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Eveindustry.Models;
using Eveindustry.StaticDataModels;

namespace Eveindustry.Tests.Utils
{
    public static class DataHelper
    {
        public static EveType NewType(long id, string nameEn)
        {
            return new EveType() {Id = id, Name = new LocalizedText() {En = nameEn}};
        }

        public static BlueprintInfo NewBp(
            int id, 
            EveType et, 
            int quantity, 
            IEnumerable<(EveType Type, int Quantity)> materials, 
            EveItemManufacturingInfo.ActivityKinds activityKind = EveItemManufacturingInfo.ActivityKinds.Manufacturing)
        {
            var activity = new BlueprintActivity()
            {
                Materials = materials
                    .Select(m => new Material()
                    {
                        Quantity = m.Quantity,
                        TypeId = (int) m.Type.Id
                    }).ToArray(),
                Products = new[] {new Material() {Quantity = quantity, TypeId = (int) et.Id}}
            };
            var bpActivities = new BlueprintActivities();
            if (activityKind == EveItemManufacturingInfo.ActivityKinds.Manufacturing)
            {
                bpActivities.Manufacturing = activity;
            }
            else
            {
                bpActivities.Reaction = activity;
            }
            var result = new BlueprintInfo()
            {
                Id = id,
                Activities = bpActivities
            };
            return result;
        }
    }
}