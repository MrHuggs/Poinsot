using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Assets.DoubleMath;

public class PlotMotion : MonoBehaviour
{
	public PRigidBody Body;

	const int MaxDataPoints = 4096;
	struct Data
	{
		public double t;
		public DVector3 Omega;
		public DVector3 L;
		public double LNorm;
		public double E;

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
		val.Omega = Body.BodyOmega;
		val.L = Body.CurrentL();
		val.LNorm = val.L.magnitude;
		val.E = Body.CurrentE();

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

fig.suptitle(ftitle, fontsize=16)

plt.plot(t, wx, label = 'wx')
plt.plot(t, wy, label = 'wy')
plt.plot(t, wz, label = 'wz')

ax.xaxis.set_major_locator(MultipleLocator(5))
ax.xaxis.set_major_formatter(FormatStrFormatter('%d'))
ax.xaxis.set_minor_locator(MultipleLocator(1))

ax.yaxis.set_major_locator(MultipleLocator(1))
ax.yaxis.set_minor_locator(MultipleLocator(.1))

ax.grid(which = 'major')

plt.xlabel('time (sec)')
plt.ylabel('omega (rad/sec)')
plt.legend()

plt.show()
";

	// Todo: Set this to be the actual python path:
	const string PythonPath = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python36_64\python.exe";

	delegate double GetValue(Data data);
	void MakeValueString(StringBuilder sb, string name, GetValue get_value)
	{
		sb.Clear();
		sb.AppendFormat("{0} = [", name);
		foreach (var data in DataPoints)
			sb.AppendFormat("{0}, ", get_value(data));
		sb.Append("]");

	}

	List<string> TempFiles = new List<string>();

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

			MakeValueString(sb, "lx", data => data.L.x);
			writer.WriteLine(sb);

			MakeValueString(sb, "ly", data => data.L.y);
			writer.WriteLine(sb);

			MakeValueString(sb, "lz", data => data.L.z);
			writer.WriteLine(sb);

			MakeValueString(sb, "ln", data => data.LNorm);
			writer.WriteLine(sb);

			MakeValueString(sb, "E", data => data.E);
			writer.WriteLine(sb);

			writer.WriteLine(string.Format("ftitle = \"Body W vs t: I=({0:F2},{1:F2},{2:F2}) W=({3:F2},{4:F2},{5:F2})\"", Body.I.x, Body.I.y, Body.I.z, 
						Body.InitialOmega.x, Body.InitialOmega.y, Body.InitialOmega.z));

			writer.Write(PythonFooter);
		}
		Debug.Log(string.Format("Using Python comand line: \"{0}\" \"{1}\"", PythonPath, fname));


		System.Diagnostics.Process p = new System.Diagnostics.Process();
		p.StartInfo.FileName = PythonPath;
		p.StartInfo.Arguments = fname;
		p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		p.Start();

		p.WaitForExit();

		TempFiles.Add(fname);
	}

	private void OnDestroy()
	{
		foreach (var fname in TempFiles)
		{
			System.IO.File.Delete(fname);
		}
	}
}
