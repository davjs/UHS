int global;
const int fynal;

extern void messy();

namespace boo
{

	void messier()
	{
		::messy();
		::messy();
	}

	int calc()
	{
		return fynal + 1;
	}
}
