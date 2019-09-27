Shader "Custom/PureColor"
{
	Properties
	{
		_OutlineColor("OutlineColor", Color) = (1,1,0,1)
	}
	SubShader
	{
		Pass
		{
			Color(1,1,0,1)
        }
    }
}
