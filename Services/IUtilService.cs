namespace Backend.Services {
    public interface IUtilService{
        public string DateTimeToString(string datetime);
        public string ConvertNumToString(int num);

        public string ConvertDateStringToString(string str);

        public string changeDateFormat(string txt) ;
    }
}
