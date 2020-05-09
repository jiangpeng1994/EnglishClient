using cn.bmob.io;
using System;

public class CanLogin : BmobTable
{
    private String fTable;
    public BmobBoolean status { get; set; }

    //构造函数
    public CanLogin() { }

    //构造函数
    public CanLogin(String tableName)
    {
        this.fTable = tableName;
    }

    public override string table
    {
        get
        {
            if (fTable != null)
            {
                return fTable;
            }
            return base.table;
        }
    }

    //读字段信息
    public override void readFields(BmobInput input)
    {
        base.readFields(input);
        
        this.status = input.getBoolean("status");
    }

    //写字段信息
    public override void write(BmobOutput output, bool all)
    {
        base.write(output, all);
        
        output.Put("status", this.status);
    }
}