using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlotMotion : MonoBehaviour
{
	public PRigidBody Body;

	const int MaxDataPoints = 4096;
	struct Data
	{
		public float t;
		public Vector3 Omega;
	}

	float TotalTime;
	List<Data> DataPoints;

	void Reset()
	{
		TotalTime = 0;
		DataPoints = new List<Data>();
		Sample();		
	}

	void Sample()
	{
		Data val;
		val.t = TotalTime;
		val.Omega = Body.BodyOmega();

		DataPoints.Add(val);
	}


	private void Awake()
	{
		Body.BodyParmsChanged += Reset;		
	}
	// Start is called before the first frame update
	void Start()
    {
		Reset();
    }

    // Update is called once per frame
    void Update()
    {
		TotalTime += Time.deltaTime;

		if (DataPoints.Count < MaxDataPoints)
			Sample();
    }

	const string PythonFooter =
@"import matplotlib.pyplot as plt
from matplotlib.ticker import MultipleLocator, FormatStrFormatter

fig, ax = plt.subplots()

fig.suptitle('Angular Velocity in Body frame vs Time', fontsize=16)

plt.plot(t, wx, label = 'wx')
plt.plot(t, wy, label = 'wy')
plt.plot(t, wz, label = 'wz')

majorLocator = MultipleLocator(5)
majorFormatter = FormatStrFormatter('%d')
minorLocator = MultipleLocator(1)

ax.xaxis.set_major_locator(majorLocator)
ax.xaxis.set_major_formatter(majorFormatter)
ax.xaxis.set_minor_locator(minorLocator)

ax.yaxis.set_major_locator(majorLocator)
ax.yaxis.set_minor_locator(minorLocator)

ax.grid(which = 'major')

plt.xlabel('time (sec)')
plt.ylabel('omega (rad/sec)')
plt.legend()

plt.show()
";

	// Todo: Set this to be the actual python path:
	const string PythonPath = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python36_64\python.exe";

	delegate float GetValue(Data data);
	void MakeValueString(StringBuilder sb, string name, GetValue get_value)
	{
		sb.Clear();
		sb.AppendFormat("{0} = [", name);
		foreach (var data in DataPoints)
			sb.AppendFormat("{0}, ", get_value(data));
		sb.Append("]");
	}


	public void Write()
	{
		string fname = System.IO.Path.GetTempFileName();

		StringBuilder sb = new StringBuilder(128 + MaxDataPoints * 64);

		using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fname))
		{
			MakeValueString(sb, "t", data => data.t);
			writer.WriteLine(sb);

			MakeValueString(sb, "wx", data => data.Omega.x);
			writer.WriteLine(sb);

			MakeValueString(sb, "wy", data => data.Omega.y);
			writer.WriteLine(sb);

			MakeValueString(sb, "wz", data => data.Omega.z);
			writer.WriteLine(sb);

			writer.Write(PythonFooter);
		}
		Debug.Log(string.Format("Wrote temp file: {0}", fname));

		System.Diagnostics.Process p = new System.Diagnostics.Process();
		p.StartInfo.FileName = PythonPath;
		p.StartInfo.Arguments = fname;
		p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		p.Start();

		p.WaitForExit();

		System.IO.File.Delete(fname);

	}
}
