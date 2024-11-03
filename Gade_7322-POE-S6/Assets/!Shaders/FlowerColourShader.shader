Shader "Custom/FlowerColourShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _SwayAmplitude ("Sway Amplitude", Float) = 0.1
        _SwayFrequency ("Sway Frequency", Float) = 1.0
        _SwayHeightPercentage ("Sway Height Percentage", Float) = 0.5
        _BlendZoneHeight ("Blend Zone Height", Float) = 0.1
        _TimeOffset ("Time Offset", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;
            float _SwayAmplitude;
            float _SwayFrequency;
            float _SwayHeightPercentage;
            float _BlendZoneHeight;
            float _TimeOffset;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;

                float maxFlowerHeight = 1.0; 
                float swayThreshold = maxFlowerHeight * _SwayHeightPercentage;
                float blendStart = swayThreshold - (maxFlowerHeight * _BlendZoneHeight);

                float swayOffset = sin(_TimeOffset * _SwayFrequency) * _SwayAmplitude;

                float blendFactor = smoothstep(blendStart, swayThreshold, v.vertex.y);

                v.vertex.x += swayOffset * blendFactor;

                o.pos = UnityObjectToClipPos(v.vertex);

                o.color = _Color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
