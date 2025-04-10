using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RWPaintingTool
{
    public struct RWPT_GraphicRequest
    {
        public RWPT_GraphicRequest(GraphicRequest req, params Color[] colors) : this (req.graphicClass, req.path, req.shader, req.drawSize, req.graphicData, req.renderQueue, req.shaderParameters, req.maskPath, colors)
        {

        }
        public RWPT_GraphicRequest(Type graphicClass, string path, Shader shader, Vector2 drawSize, GraphicData graphicData, int renderQueue, List<ShaderParameter> shaderParameters, string maskPath, params Color[] colors)
        {
            this.graphicClass = graphicClass;
            this.path = path;
            this.maskPath = maskPath;
            this.shader = shader;
            this.drawSize = drawSize;
            this.colors = colors ?? [];
            this.graphicData = graphicData;
            this.renderQueue = renderQueue;
            this.shaderParameters = (shaderParameters.NullOrEmpty<ShaderParameter>() ? null : shaderParameters);
            this.texture = null;

        }

        public RWPT_GraphicRequest(Type graphicClass, Texture2D texture, Shader shader, Vector2 drawSize, GraphicData graphicData, int renderQueue, List<ShaderParameter> shaderParameters, string maskPath, params Color[] colors)
        {
            this.graphicClass = graphicClass;
            this.texture = texture;
            this.maskPath = maskPath;
            this.shader = shader;
            this.drawSize = drawSize;
            this.colors = colors ?? [];
            this.graphicData = graphicData;
            this.renderQueue = renderQueue;
            this.shaderParameters = (shaderParameters.NullOrEmpty<ShaderParameter>() ? null : shaderParameters);
            this.path = null;
        }

        public override int GetHashCode()
        {
            if (this.path == null)
            {
                this.path = BaseContent.BadTexPath;
            }
            return
                Gen.HashCombine(
                    Gen.HashCombine<List<ShaderParameter>>(Gen.HashCombine<int>(Gen.HashCombine<GraphicData>(Gen.HashCombineStruct<Color>(Gen.HashCombineStruct<Color>(Gen.HashCombineStruct<Vector2>(Gen.HashCombine<Shader>(Gen.HashCombine<string>(Gen.HashCombine<Texture2D>(Gen.HashCombine<string>(Gen.HashCombine<Type>(0, this.graphicClass), this.path), this.texture), this.maskPath), this.shader), this.drawSize), this.colors.ElementAtOrDefault(0)), this.colors.ElementAtOrDefault(1)), this.graphicData), this.renderQueue), this.shaderParameters),
                    colors);

        }

        public override string ToString()
        {
            return $"{graphicClass}: {path} - {maskPath}\n{string.Join(", ", colors)}";
            return string.Format("{0}: {1}", this.graphicClass, this.path);
        }

        public override bool Equals(object obj)
        {
            return obj is RWPT_GraphicRequest && this.Equals((RWPT_GraphicRequest)obj);
        }

        public bool Equals(RWPT_GraphicRequest other)
        {
            return this.graphicClass == other.graphicClass && this.path == other.path && this.texture == other.texture && this.maskPath == other.maskPath && this.shader == other.shader && this.drawSize == other.drawSize && 
                this.graphicData == other.graphicData && this.renderQueue == other.renderQueue && this.shaderParameters == other.shaderParameters && Enumerable.SequenceEqual(colors, other.colors);
        }

        public static implicit operator GraphicRequest(RWPT_GraphicRequest req)
        {
            return new GraphicRequest(req.graphicClass, req.path, req.shader, req.drawSize, req.colors.ElementAtOrDefault(0), req.colors.ElementAtOrDefault(1), req.graphicData, req.renderQueue, req.shaderParameters, req.maskPath);
        }


        public Type graphicClass;

        public Texture2D texture;

        public string path;

        public string maskPath;

        public Shader shader;

        public Vector2 drawSize;

        public Color[] colors; 

        public GraphicData graphicData;

        public int renderQueue;

        public List<ShaderParameter> shaderParameters;
    }
}