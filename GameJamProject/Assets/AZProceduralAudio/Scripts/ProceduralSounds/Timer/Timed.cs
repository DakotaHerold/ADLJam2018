public interface Timed {
	bool Repeating { get; }
	float ElapsedTime { get; }
	float Delay { get; set; }
}