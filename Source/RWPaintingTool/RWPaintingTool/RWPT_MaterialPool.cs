using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWPaintingTool
{
    static class RWPT_MaterialPool
    {
        private static Dictionary<RWPT_MaterialRequest, Material> matDictionary = new Dictionary<RWPT_MaterialRequest, Material>();

        public static Material MatFrom(RWPT_MaterialRequest req)
        {
            Log.Message(req);

            if (!UnityData.IsInMainThread)
            {
                Log.Error("Tried to get a material from a different thread.");
                return null;
            }
            if (req.mainTex == null && req.needsMainTex)
            {
                Log.Error("MatFrom with null sourceTex.");
                return BaseContent.BadMat;
            }
            if (req.shader == null)
            {
                Log.Warning("Matfrom with null shader.");
                return BaseContent.BadMat;
            }
            if (req.maskTex != null && !req.shader.SupportsMaskTex())
            {
                Log.Error("MaterialRequest has maskTex but shader does not support it. req=" + req.ToString());
                req.maskTex = null;
            }
            req.colors = req.colors.Clone() as Color[];
            Material material;
            if (!matDictionary.TryGetValue(req, out material))
            {
                material = MaterialAllocator.Create(req.shader);
                material.name = req.shader.name;
                if (req.mainTex != null)
                {
                    Material material2 = material;
                    material2.name = material2.name + "_" + req.mainTex.name;
                    material.mainTexture = req.mainTex;
                }
                material.color = req.colors[0];
                if (req.maskTex != null)
                {
                    material.SetTexture(ShaderPropertyIDs.MaskTex, req.maskTex);
                    material.SetColor(ShaderPropertyIDs.ColorTwo, req.colors[1]);
                    material.SetColor("_ColorThree", req.colors.ElementAtOrDefault(2));
                    material.SetColor("_ColorFour", req.colors.ElementAtOrDefault(3));
                    material.SetColor("_ColorFive", req.colors.ElementAtOrDefault(4));
                    material.SetColor("_ColorSix", req.colors.ElementAtOrDefault(5));
                }
                if (req.renderQueue != 0)
                {
                    material.renderQueue = req.renderQueue;
                }
                if (!req.shaderParameters.NullOrEmpty<ShaderParameter>())
                {
                    for (int i = 0; i < req.shaderParameters.Count; i++)
                    {
                        req.shaderParameters[i].Apply(material);
                    }
                }
                matDictionary.Add(req, material);
                //matDictionaryReverse.Add(material, req);
                //if (req.shader == ShaderDatabase.CutoutPlant || req.shader == ShaderDatabase.TransparentPlant)
                //{
                //    WindManager.Notify_PlantMaterialCreated(material);
                //}
            }
            return material;
        }
    }
}
