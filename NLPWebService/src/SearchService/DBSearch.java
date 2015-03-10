package SearchService;

import getBaiduMusic.Music;
import getBaiduMusic.SearchMusic;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;

import LoadLocalSource.DBConn;

import com.mongodb.DBCursor;
import com.mongodb.DBObject;



public class DBSearch {
	private SearchMusic getMusic;
	public DBSearch(){
		getMusic = new SearchMusic();
	}
        public static void main(String[] args)
        {
            DBSearch dbs = new DBSearch();
        }
	 public String SearchGenre(String genreVerb, String songN) {
    	     DBConn knowledge = new DBConn();
             DBCursor cur = null;
             Music music = null;
             String url = "";
             String category = "";
             String songName = "";
             String singer = "";
             if (!songN.equals("")) {
                 //cur = knowledge.search("mediaCollection","Metadata.Genre", genreVerb, "Metadata.Name", songN);
                 category = "MUSIC";
                 music = getMusic.searchMusic(songN, "");
                 songName = songN;
                 singer = genreVerb;
             } else {
                 cur = knowledge.search("mediaCollection", "Metadata.Genre", genreVerb);
                 if (cur == null) {
                     return "$";
                 }
                 int b = (int) (Math.random() * (cur.count() / 2));//产生0-count/2的整数随机数　
                 for (int i = 0; i < b; i++) {//产生随机的效果
                     cur.next();
                 }
                 while (cur.hasNext()) {
                     songName = "";
                     singer = "";
                     url = "";
                     category = "";
                     DBObject searchResult = cur.next();
                     Object temp;
                     DBObject metadata;
                     temp = searchResult.get("Category");
                     if (temp != null) {
                         category = temp.toString();
                     }
                     metadata = (DBObject) searchResult.get("Metadata");
                     if (metadata != null) {
                         songName = metadata.get("Name").toString();
                         singer = metadata.get("Singer").toString();
                     }
                     music = getMusic.searchMusic(songName, "");
                     if (music != null && music.getHQMusicUrl() != null) {
                    	 break;
                     }
                 }                 
             }
             url = music.getHQMusicUrl();
             if (url != null && !url.equals("null") && !url.equals("") && !url.equals("NULL")) {
                 return category.toUpperCase() + "$" + url + "$" + singer + "-" + songName;
             }
             return "$";
             // TODO Auto-generated method stub
	}
	public String RandomSearchOneWordWithCategory(String cate) {
        DBConn knowledge = new DBConn();
        DBCursor cur =    knowledge.search("mediaCollection","Category", cate);
        String category = "";
        String url = "";
        String songName = "";
        String singer = "";
        int b=(int)(Math.random()*(cur.count()/2));//产生0-count/2的整数随机数　
        for(int i = 0 ;i < b;i++){//产生随机的效果
            cur.next();
        }
        while(cur.hasNext()){           
            DBObject searchResult = cur.next();        
             Object temp;
            temp = searchResult.get("Category");
            if(temp!= null)
                category = temp.toString();
            temp = searchResult.get("URL");
            if(temp != null)
                url =temp.toString(); 
            
            
            temp = searchResult.get("Name");
            if( temp != null)
                songName = temp.toString();
                
            if(category.equals("story"))
               singer = "故事";
            else if (category.equals("radio"))
               singer = "广播";                            
        
            if(!url.equals("null"))
            {
                return category.toUpperCase() + "$" + url + "$" + singer + "-" + songName;
            }
        }
        return "$";
        //throw new UnsupportedOperationException("Not supported yet."); //To change body of generated methods, choose Tools | Templates.
    }
    
    public String RandomSearchMusicFromBaidu()
    {
        DBConn knowledge = new DBConn();
        DBCursor cur =    knowledge.search("mediaCollection","Category", "music");
        String category = "";
        String url = "";
        String songName = "";
        String singer = "";
        if(cur == null)
        	return "$";
        int b=(int)(Math.random()*(cur.count()/2));//产生0-count/2的整数随机数　
        for(int i = 0 ;i < b;i++){//产生随机的效果
            cur.next();
        }
        while(cur.hasNext()){           
            DBObject searchResult = cur.next();
             Object temp;
             DBObject metadata;
            temp = searchResult.get("Category");
            if(temp!= null)
                category = temp.toString();
            
            metadata = (DBObject) searchResult.get("Metadata");            
            if(metadata != null){
                songName = metadata.get("Name").toString();
                 singer = metadata.get("Singer").toString();
            }     
            Music music  =  getMusic.searchMusic(songName, singer);
            if(music != null)
                url = music.getHQMusicUrl();
            
            if(!url.equals("null") && !url.equals("") && url != null && !url.equals("NULL"))
            {
                return category.toUpperCase() + "$" + url + "$" + singer + "-" + songName;
            }
        }
        return "$";
    }
    public String RandomSearchMusicFromBaidu(String songName, String singer)
    {
        DBConn knowledge = new DBConn();
        DBCursor cur = null;
        Music music = null;
        if(!singer.equals("") && !songName.equals(""))
        {
            music  =  getMusic.searchMusic(songName, singer);
            if(music == null)
                return "$";     
            else
                return "MUSIC$" + music.getHQMusicUrl()+"$"+singer + "-" + songName;
        }
        if(singer.equals("") && !songName.equals(""))           
            cur =  knowledge.search("mediaCollection","Name", songName);
        if(!singer.equals("") && songName.equals(""))
            cur = knowledge.search("mediaCollection","Metadata.Singer", singer);
        String category = "";
        String url = "";
        if(cur == null)
        	return "$";
        int b=(int)(Math.random()*(cur.count()/2));//产生0-count/2的整数随机数　
        for(int i = 0 ;i < b;i++){//产生随机的效果
        	DBObject noUseResult =  cur.next();
        }
        while(cur.hasNext()){           
            DBObject searchResult = cur.next();
             Object temp;
             DBObject metadata;
            temp = searchResult.get("Category");
            if(temp!= null)
                category = temp.toString();
            
            metadata = (DBObject) searchResult.get("Metadata");            
            if(metadata != null && category.equals("music")){
                songName = metadata.get("Name").toString();
                singer = metadata.get("Singer").toString();
                music  =  getMusic.searchMusic(songName, singer);
                if(music != null)
                    url = music.getHQMusicUrl();
            }     
            else if(metadata != null && (category.equals("story") || category.equals("radio")))
            {
                temp = searchResult.get("Name");
                if( temp != null)
                    songName = temp.toString();
                temp = searchResult.get("URL");
                if( temp != null)
                    url= temp.toString();
                if(category.equals("story"))
                    singer = "故事";
                else if (category.equals("radio"))
                    singer = "广播";                            
            }
            
            if(!url.equals("null") && !url.equals("") && url != null && !url.equals("NULL"))
            {
                return category.toUpperCase() + "$" + url + "$" + singer + "-" + songName;
            }
        }
        return "$";
    }

    public String SearchOneWord(String noLast ,String targetCollection) {
        DBConn knowledge = new DBConn();
        DBCursor cur =    knowledge.search("mediaCollection","Name", noLast);
        String category = "";
        String url = "";
        String singer = "";
        String songName = "";
        while(cur.hasNext()){
            DBObject searchResult = cur.next();
             Object temp;
            temp = searchResult.get("Category");
            if(temp!= null)
                category = temp.toString();
            temp = searchResult.get("URL");
            if(temp != null)
                url =temp.toString(); 
            songName = noLast;
            if(category.equals("story"))
                singer = "故事";
            else if (category.equals("radio"))
                singer = "广播";                                            
            if(category.equals(targetCollection)){
                if(!url.equals("null"))
                    return category.toUpperCase() + "$" + url + "$" + singer + "-" + songName;
            }
        }
        return "$";
    }

    public String searchCommand(String verb) {
		DBConn knowledge = new DBConn();
		DBCursor cur = knowledge.search("commandCollection", "Name", verb);
		String command = null;
		while (cur.hasNext()) {
			DBObject searchResult = cur.next();
			Object temp;
			temp = searchResult.get("Command");
			if (temp != null)
				command = temp.toString();
			return command;
		}
		return command;
    }

    public String RandomSearchOneWordWithSinger(String noLast, String music, String target) {
         DBConn knowledge = new DBConn();
        DBCursor cur =    knowledge.search("mediaCollection",target, noLast);
       String category = "";
        String url = "";
         int b=(int)(Math.random()*(cur.count()/2));//产生0-count/2的整数随机数　
        for(int i = 0 ;i < b;i++){//产生随机的效果
            cur.next();
        }
        while(cur.hasNext()){
             DBObject searchResult = cur.next();
             Object temp;
            temp = searchResult.get("Category");
            if(temp!= null)
                category = temp.toString();
            temp = searchResult.get("URL");
            if(temp != null)
                url =temp.toString();    
            if(category.equals("music") && url != null && !url.equals("") && !url.equals("NULL") && !url.equals("null")){
                return category.toUpperCase() + "$" + url;
            }
        }
        
        return "$";
    }

   public String SearchOneWordSingerAndName(String afterVerbSinger, String afterVerbSong) {
       DBConn knowledge = new DBConn();
        DBCursor cur =    knowledge.search("mediaCollection","Metadata.Singer", afterVerbSinger,"Metadata.Name", afterVerbSong);
      
        String category = "";
        String url = "";
        System.out.println(cur.count());
 
        while(cur.hasNext()){
             DBObject searchResult = cur.next();
             Object temp;
            temp = searchResult.get("Category");
            if(temp!= null)
                category = temp.toString();
            temp = searchResult.get("URL");
            if(temp != null)
                url =temp.toString();    
            if(category.equals("music") && url != null && !url.equals("") && !url.equals("NULL") && !url.equals("null")){
                return category.toUpperCase() + "$" + url;
            }
        }
        return "$";
    }

   public String SearchOneWordInDict(String bin) {
       DBConn knowledge = new DBConn();
        DBCursor cur =    knowledge.search("dictCollection","Name", bin);
        String content = "";
        String pron = "";
        while(cur.hasNext()){
            DBObject searchResult = cur.next();
            Object temp;
            temp = searchResult.get("Content");
            if(temp!= null)
                content = temp.toString();
            temp = searchResult.get("Pronunciation");
            if(temp != null)
                pron =temp.toString();    
           return content;
        }
        return  null;
    }

	public String SearchRadioCategory(String noLast) {
		DBConn knowledge = new DBConn();
		DBCursor curCount = knowledge.search("mediaCollection", "Category",
				"radio");
		int resultCount = 0;
		while (curCount.hasNext()) {
			DBObject searchResult = curCount.next();
			Object temp;
			DBObject metadata;
			String radioCategory = "";
			metadata = (DBObject) searchResult.get("Metadata");
			if (metadata != null) {
				temp = metadata.get("RadioCategory");
				if (temp != null) {
					radioCategory = temp.toString().trim();
				}
			}
			if (radioCategory != null && radioCategory.equals(noLast))
				resultCount++;
		}
		
		DBCursor cur = knowledge.search("mediaCollection", "Category", "radio");
		if (cur == null)
			return "$";
		int b = (int) (Math.random() * (resultCount / 2 + 1));// 产生0-count/2的整数随机数　
		String category = "";
		String url = "";
		String radioName = "";
		int count = 0;
		while (cur.hasNext()) {
			DBObject searchResult = cur.next();
			Object temp;
			DBObject metadata;
			String radioCategory = "";
			temp = searchResult.get("Category");
			if (temp != null)
				category = temp.toString();
			temp = searchResult.get("URL");
			if (temp != null)
				url = temp.toString();

			temp = searchResult.get("Name");
			if (temp != null)
				radioName = temp.toString();

			metadata = (DBObject) searchResult.get("Metadata");
			if (metadata != null) {
				temp = metadata.get("RadioCategory");
				if (temp != null)
					radioCategory = temp.toString().trim();
			}
			if (radioCategory != null && radioCategory.equals(noLast)) {
				count++;
				if (count > b) {
					if (url != null && !url.equals("null") && !url.equals("")
							&& !url.equals("NULL")) {
						return category.toUpperCase() + "$" + url + "$"
								+ radioName;
					}
				}
			}
		}
		return "$";
	}
	public String searchCommand2(String verb) {
		DBConn knowledge = new DBConn();
		DBCursor cur = knowledge.search("commandCollection", "Name", verb);
		String command = null;
		while (cur.hasNext()) {
			DBObject searchResult = cur.next();
			Object temp;
			temp = searchResult.get("Command");
			if (temp != null)
				command = temp.toString();
			return command;
		}
		return command;
	}
	    
}

