//Usage : > result.exe exp.txt out.txt res.txt log.txt

#include<iostream>
#include<fstream>
#include<stdlib.h>
#include<stdio.h>

using namespace std;

int isSuc(char file[])
{
    ifstream log(file);
    string s;
    log>>s;
    //cout<<s;
    if(s=="Successful.")
    {
        return 1;
    }
    else if(s=="TimeLimitExceed.")
    {
        return 2;
    }
    else if(s=="RunTimeError.")
    {
        return 3;
    }
}

int main(int argc, char *argv[])
{
    if(argc!=5)
    {
        cerr<<"Invalid Syantax.";
        exit(1);
    }

    int ty=isSuc(argv[4]);
    //1 sucess
    //2 tle
    //3 re
    
    bool ac=true;
    ifstream exp(argv[1]);
    ifstream out(argv[2]);
    ofstream res(argv[3]);
    
    if(ty==2)
    {
        res<<"Time Limit Exceed.";
        res.close();
        return 0;
    }
    else if(ty==3)
    {
        res<<"Run Time Error.";
        res.close();
        return 0;
    }
    
    char x,y;
    x=exp.get();
    y=out.get();

    res<<"WA\nTest Case Failed - Check Difference\n";

    while(x!=-1&&y!=-1)
    {
        if(x!=y)
        {
            ac=false;
            res<<"__"<<x<<","<<y<<"\n";
            break;
        }
        res<<x;
        x=exp.get();
        y=out.get();
        
        while(x==' '||x=='\t'||x=='\n')
        {
            x=exp.get();
        }
        
        while(y==' '||y=='\t'||y=='\n')
        {
            y=out.get();
        }
    }

    if(x!=y&&ac)
    {
        ac=false;
        res<<"__";
        if(x==EOF)
        {
            res<<"EOF";
        }
        else
        {
            res<<x;
        }

        res<<",";

        if(y==EOF)
        {
            res<<"EOF";
        }
        else
        {
            res<<y;
        }
        res<<"\n";
    }

    exp.close();
    out.close();
    res.close();

    if(ac==false)
    {
        return 0;
    }

    remove(argv[3]);
    ofstream RES(argv[3]);
    RES<<"AC.\nTest Case Passed";
    RES.close();
    return 0;
}

