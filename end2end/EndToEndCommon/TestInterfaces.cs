using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    public interface IOneTester
    {
        Task TestOne();
    }

    public interface IManyTester
    {
        Task TestMany();
    }

    public interface IExecTester
    {
        Task TestExec();
    }

    public interface IExecRowsTester
    {
        Task TestExecRows();
    }

    public interface IExecLastIdTester
    {
        Task TestExecLastId();
    }


    public interface ICopyFromTester
    {
        Task TestCopyFrom();
    }
}