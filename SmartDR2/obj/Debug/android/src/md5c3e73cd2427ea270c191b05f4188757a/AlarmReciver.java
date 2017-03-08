package md5c3e73cd2427ea270c191b05f4188757a;


public class AlarmReciver
	extends android.content.BroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onReceive:(Landroid/content/Context;Landroid/content/Intent;)V:GetOnReceive_Landroid_content_Context_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("SmartDR2.AlarmReciver, SmartDR2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", AlarmReciver.class, __md_methods);
	}


	public AlarmReciver () throws java.lang.Throwable
	{
		super ();
		if (getClass () == AlarmReciver.class)
			mono.android.TypeManager.Activate ("SmartDR2.AlarmReciver, SmartDR2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onReceive (android.content.Context p0, android.content.Intent p1)
	{
		n_onReceive (p0, p1);
	}

	private native void n_onReceive (android.content.Context p0, android.content.Intent p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
