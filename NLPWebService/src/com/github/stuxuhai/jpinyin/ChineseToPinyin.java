/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.github.stuxuhai.jpinyin;

import LoadLocalSource.GetSource;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.Vector;

/**
 *
 * @author v-feiche
 */
public class ChineseToPinyin {
    
    public Vector<String> startVerbPinyin = new Vector<String>();
    public Vector<String> stopVerbPinyin = new Vector<String>();
    public Vector<String> openVerbPinyin = new Vector<String>();
    public Vector<String> closeVerbPinyin = new Vector<String>();
    public Vector<String> searchVerbPinyin = new Vector<String>();
    public Vector<String> changeVerbPinyin = new Vector<String>();
    public Vector<String> upVerbPinyin = new Vector<String>();
    public Vector<String> downVerbPinyin = new Vector<String>();
    String sentencePinyin = "";
    String sentence = "";
    
    public static void main(String[] args)
    {
        String words = "停";  
        String pinyin = null;         
        ChineseToPinyin ctp = new ChineseToPinyin();
        pinyin = ctp.FindAndResolveVerb(words);
        System.out.println(pinyin);
    }

    public ChineseToPinyin()
    {
        GetSource gs  = new GetSource(); 
        Vector<String> startVerb = gs.GetStartVerb();
        Vector<String> stopVerb = gs.GetStopVerb();
        Vector<String> openVerb = gs.GetOpenVerb();
        Vector<String> closeVerb = gs.GetCloseVerb();
        Vector<String> searchVerb = gs.GetSearchVerb();
        Vector<String> changeVerb = gs.GetChangeVerb();
        Vector<String> upVerb = gs.GetUpVerb();
        Vector<String> downVerb = gs.GetDownVerb();
        
        Collections.sort(startVerb, new Comparator() {
            public int compare(Object a, Object b) {
                return ( new Integer(((String) b).length()) ).compareTo( new Integer(((String) a).length()));
            }
        });
        Collections.sort(stopVerb, new Comparator() {
            public int compare(Object a, Object b) {
                return ( new Integer(((String) b).length()) ).compareTo( new Integer(((String) a).length()));
            }
        });
        Collections.sort(openVerb, new Comparator() {
            public int compare(Object a, Object b) {
                return ( new Integer(((String) b).length()) ).compareTo( new Integer(((String) a).length()));
            }
        });
        Collections.sort(closeVerb, new Comparator() {
            public int compare(Object a, Object b) {
                return ( new Integer(((String) b).length()) ).compareTo( new Integer(((String) a).length()));
            }
        });
        Collections.sort(searchVerb, new Comparator() {
            public int compare(Object a, Object b) {
                return ( new Integer(((String) b).length()) ).compareTo( new Integer(((String) a).length()));
            }
        });
        Collections.sort(changeVerb, new Comparator() {
            public int compare(Object a, Object b) {
                return ( new Integer(((String) b).length()) ).compareTo( new Integer(((String) a).length()));
            }
        });
        Collections.sort(upVerb, new Comparator() {
            public int compare(Object a, Object b) {
                return ( new Integer(((String) b).length()) ).compareTo( new Integer(((String) a).length()));
            }
        });
        Collections.sort(downVerb, new Comparator() {
            public int compare(Object a, Object b) {
                return ( new Integer(((String) b).length()) ).compareTo( new Integer(((String) a).length()));
            }
        });
        //System.out.println(startVerb);
        for(int i = 0; i < startVerb.size(); i++)
        {
            startVerbPinyin.add(ConvertChinese(startVerb.get(i)));
        }
        for(int i = 0; i < stopVerb.size(); i++)
        {
            stopVerbPinyin.add(ConvertChinese(stopVerb.get(i)));
        }
        for(int i = 0; i < openVerb.size(); i++)
        {
            openVerbPinyin.add(ConvertChinese(openVerb.get(i)));
        }
        for(int i = 0; i < closeVerb.size(); i++)
        {
            closeVerbPinyin.add(ConvertChinese(closeVerb.get(i)));
        }
        for(int i = 0; i < searchVerb.size(); i++)
        {
            searchVerbPinyin.add(ConvertChinese(searchVerb.get(i)));
        }
        for(int i = 0; i < changeVerb.size(); i++)
        {
            changeVerbPinyin.add(ConvertChinese(changeVerb.get(i)));
        }
        for(int i = 0; i < upVerb.size(); i++)
        {
            upVerbPinyin.add(ConvertChinese(upVerb.get(i)));
        }
        for(int i = 0; i < downVerb.size(); i++)
        {
            downVerbPinyin.add(ConvertChinese(downVerb.get(i)));
        }
        //System.out.println(startVerbPinyin);
    }
    
    public String FindAndResolveVerb(String words)
    {
        sentencePinyin = ConvertChinese(words);  
        if(sentencePinyin.equals("ting"))
            return "停";
        sentence = words;
        ArrayList<String[]> myList = new ArrayList<String[]>();
        myList.add(SearchStartVerb().split(","));
        myList.add(SearchStopVerb().split(","));
        myList.add(SearchOpenVerb().split(","));
        myList.add(SearchCloseVerb().split(","));
        myList.add(SearchLookupVerb().split(","));
        myList.add(SearchChangeVerb().split(","));
        myList.add(SearchUpVerb().split(","));
        myList.add(SearchDownVerb().split(","));        
        int indMin = 10000;
        String resMin = words;
        for(int i = 0; i < myList.size(); i++)
        {
            int ind = Integer.parseInt(myList.get(i)[0]);
            if(ind != -1 && ind < indMin)
            {
                indMin = ind;
                resMin = myList.get(i)[1];
            }
        }
        return resMin;
    }
    
    public String ConvertChinese(String words)
    {
        final String separator = " ";    
        String pinyin = null;
        pinyin = PinyinHelper.convertToPinyinString(words, separator, PinyinFormat.WITHOUT_TONE); 
        return pinyin;
    }
    
    public String SearchStartVerb()
    {
        //String sentencePinyin = ConvertChinese(sentence);
        for(int i = 0; i < startVerbPinyin.size(); i++)
        {
            String sent = startVerbPinyin.get(i);
            String[] strVerb = sent.split(" ");
            int index = sentencePinyin.indexOf(sent);
            if(index != -1)
            {              
                String replaceSentence = "";
                if(index > 0)
                {
                    String []str = sentencePinyin.substring(0, index).split(" ");
                    replaceSentence = index + "," + sentence.substring(0, str.length) + "播放" + sentence.substring(str.length+strVerb.length);
                }                 
                else
                {                
                    replaceSentence = index + "," + "播放" + sentence.substring(strVerb.length);
                }
                return replaceSentence;
            }
        }
        return (-1) + "," + sentence;
    }
    
    public String SearchStopVerb()
    {
        //String sentencePinyin = ConvertChinese(sentence);
        for(int i = 0; i < stopVerbPinyin.size(); i++)
        {
            String sent = stopVerbPinyin.get(i);
            int index = sentencePinyin.indexOf(sent);
            if(index != -1)
            {       
                String replaceSentence = index + "," + "停止播放";
                return replaceSentence;
            }
        }
        return (-1) + "," + sentence;
    }
    
    public String SearchOpenVerb()
    {
        //String sentencePinyin = ConvertChinese(sentence);
        for(int i = 0; i < openVerbPinyin.size(); i++)
        {
            String sent = openVerbPinyin.get(i);
            String[] strVerb = sent.split(" ");
            int index = sentencePinyin.indexOf(sent);
            if(index != -1)
            {
                String replaceSentence = "";
                if(index > 0)
                {
                    String []str = sentencePinyin.substring(0, index).split(" ");
                    replaceSentence = index + "," + sentence.substring(0, str.length) + "打开" + sentence.substring(str.length+strVerb.length);
                }                 
                else
                {                
                    replaceSentence = index + "," + "打开" + sentence.substring(strVerb.length);
                }
                return replaceSentence;
            }
        }
        return (-1) + "," + sentence;
    }
    
    public String SearchCloseVerb()
    {
        //String sentencePinyin = ConvertChinese(sentence);
        for(int i = 0; i < closeVerbPinyin.size(); i++)
        {
            String sent = closeVerbPinyin.get(i);
            String[] strVerb = sent.split(" ");
            int index = sentencePinyin.indexOf(sent);
            if(index != -1)
            {
                String replaceSentence = "";
                if(index > 0)
                {
                    String []str = sentencePinyin.substring(0, index).split(" ");
                    replaceSentence = index + "," + sentence.substring(0, str.length) + "关闭" + sentence.substring(str.length+strVerb.length);
                }                 
                else
                {                
                    replaceSentence = index + "," + "关闭" + sentence.substring(strVerb.length);
                }
                return replaceSentence;
            }
        }
        return (-1) + "," + sentence;
    }
    
    public String SearchLookupVerb()
    {
        //String sentencePinyin = ConvertChinese(sentence);
        for(int i = 0; i < searchVerbPinyin.size(); i++)
        {
            String sent = searchVerbPinyin.get(i);
            String[] strVerb = sent.split(" ");
            int index = sentencePinyin.indexOf(sent);
            if(index != -1)
            {
                String replaceSentence = "";
                if(index > 0)
                {
                    String []str = sentencePinyin.substring(0, index).split(" ");
                    replaceSentence = index + "," + sentence.substring(0, str.length) + "查询" + sentence.substring(str.length+strVerb.length);
                }                 
                else
                {                
                    replaceSentence = index + "," + "查询" + sentence.substring(strVerb.length);
                }
                return replaceSentence;
            }
        }
        return (-1) + "," + sentence;
    }
    public String SearchChangeVerb()
    {
        //String sentencePinyin = ConvertChinese(sentence);
        for(int i = 0; i < changeVerbPinyin.size(); i++)
        {
            String sent = changeVerbPinyin.get(i);
            String[] strVerb = sent.split(" ");
            int index = sentencePinyin.indexOf(sent);
            if(index != -1)
            {                
                String replaceSentence = "";
                if(index > 0)
                {
                    String []str = sentencePinyin.substring(0, index).split(" ");
                    replaceSentence = index + "," + sentence.substring(0, str.length) + "调至" + sentence.substring(str.length+strVerb.length);
                }                 
                else
                {                
                    replaceSentence = index + "," + "调至" + sentence.substring(strVerb.length);
                }
                return replaceSentence;
            }
        }
        return (-1) + "," + sentence;
    }
    public String SearchUpVerb()
    {
        //String sentencePinyin = ConvertChinese(sentence);
        for(int i = 0; i < upVerbPinyin.size(); i++)
        {
            String sent = upVerbPinyin.get(i);
            String[] strVerb = sent.split(" ");
            int index = sentencePinyin.indexOf(sent);
            if(index != -1)
            {
                String replaceSentence = "";
                if(index > 0)
                {
                    String []str = sentencePinyin.substring(0, index).split(" ");
                    replaceSentence = index + "," + sentence.substring(0, str.length) + "调高" + sentence.substring(str.length+strVerb.length);
                }                 
                else
                {                
                    replaceSentence = index + "," + "调高" + sentence.substring(strVerb.length);
                }
                return replaceSentence;
            }
        }
        return (-1) + "," + sentence;
    }
    public String SearchDownVerb()
    {
        //String sentencePinyin = ConvertChinese(sentence);
        for(int i = 0; i < downVerbPinyin.size(); i++)
        {
            String sent = downVerbPinyin.get(i);
            String[] strVerb = sent.split(" ");
            int index = sentencePinyin.indexOf(sent);
            if(index != -1)
            {
                String replaceSentence = "";
                if(index > 0)
                {
                    String []str = sentencePinyin.substring(0, index).split(" ");
                    replaceSentence = index + "," + sentence.substring(0, str.length) + "调低" + sentence.substring(str.length+strVerb.length);
                }                 
                else
                {                
                    replaceSentence = index + "," + "调低" + sentence.substring(strVerb.length);
                }
                return replaceSentence;
            }
        }
        return (-1) + "," + sentence;
    }
}
