//Usage : > result.exe exp.txt out.txt res.txt

#include<iostream>
#include<fstream>
#include<stdlib.h>
#include<stdio.h>

using namespace std;

int main(int argc, char *argv[])
{
    if(argc!=4)
    {
        cerr<<"Invalid Syantax";
        exit(1);
    }
    
    bool ac=true;
    ifstream exp(argv[1]);
    ifstream out(argv[2]);
    ofstream res(argv[3]);
    
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

