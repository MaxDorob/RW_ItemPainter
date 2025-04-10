using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWPaintingTool
{
    public static class RWPT_GraphicDatabase
    {
        private static Dictionary<RWPT_GraphicRequest, Graphic> allGraphics = new Dictionary<RWPT_GraphicRequest, Graphic>();
        private static Dictionary<Type, Func<GraphicRequest, Graphic>> cachedGraphicGetters = new Dictionary<Type, Func<GraphicRequest, Graphic>>();
        public static Graphic Get(RWPT_GraphicRequest req)
        {
            try
            {
                Log.Message($"Trying to get getter:\n{req}");
                Func<GraphicRequest, Graphic> func;
                if (!cachedGraphicGetters.TryGetValue(req.graphicClass, out func))
                {
                    MethodInfo method = AccessTools.Method(typeof(RWPT_GraphicDatabase),nameof(GetInner), generics: (new Type[]
                    {
                        req.graphicClass
                    }));
                    func = (Func<GraphicRequest, Graphic>)Delegate.CreateDelegate(typeof(Func<GraphicRequest, Graphic>), method);
                    cachedGraphicGetters.Add(req.graphicClass, func);
                }
                return func(req);
            }
            catch (Exception ex)
            {
                Log.Error(string.Concat(new object[]
                {
                    "Exception getting ",
                    req.graphicClass,
                    " at ",
                    req.path,
                    ": ",
                    ex.ToString()
                }));
            }
            return BaseContent.BadGraphic;
        }

        internal static T GetInner<T>(RWPT_GraphicRequest req) where T : Graphic, new()
        {
            Log.Message($"Trying to get {req}");
            req.renderQueue = ((req.renderQueue == 0 && req.graphicData != null) ? req.graphicData.renderQueue : req.renderQueue);
            Graphic graphic;
            if (!allGraphics.TryGetValue(req, out graphic))
            {
                if (!UnityData.IsInMainThread)
                {
                    Log.ErrorOnce(string.Format("Attempted to load a graphic off the main thread: {0}", req), req.GetHashCode());
                    return default(T);
                }
                graphic = Activator.CreateInstance<T>();
                graphic.Init(req);
                



                allGraphics.Add(req, graphic);
                GraphicDatabase.allGraphics.Add(req, graphic);
            }
            return (T)((object)graphic);
        }
    }
}
