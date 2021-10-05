namespace Backend.Services {
    public class UtilService: IUtilService{
        public string DateTimeToString(string datetime) {
            string[] word = datetime.Split(' ')[0].Split('/');
            string month = this.ConvertDateStringToString(word[1]) ;
            string date = this.ConvertDateStringToString(word[0]) ;
            return word[2] + "-" + month + "-" +  date ;
        }

        public string ConvertDateStringToString(string str) {
            if(str.Length < 2) return "0" + str;
            else return str;
        }

        public string ConvertNumToString(int num){
            if (num > 10) return num.ToString() ;
            else return "0" + num.ToString() ;
        }

        public string changeDateFormat(string txt){
            string[] word = txt.Split('-') ;
            return word[0] + word[2] + word[1] ;
        }
    }
}
