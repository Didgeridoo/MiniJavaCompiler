class LinearSearch{
    public static void main(String[] a){
	System.out.println(new LS().Start(10));
    }
}


// This class contains an array of integers and
// methods to initialize, print and search the array
// using Linear Search
class LS {
    int[] number ;
    int size ;
    
    // Invoke methods to initialize, print and search
    // for elements on the array
    public int Start(int sz){
	int aux012 ;
	int aux022 ;

	aux012 = this.Init(sz);
	aux022 = this.Print();
	System.out.println(9999);
	System.out.println(this.Search(8));
	System.out.println(this.Search(12)) ;
	System.out.println(this.Search(17)) ;
	System.out.println(this.Search(50)) ;
	return 55 ;
    }

    // Print array of integers
    public int Print(){
	int j2 ;

	j2 = 1 ;
	while (j2 < (size)) {
	    System.out.println(number[j2]);
	    j2 = j2 + 1 ;
	}
	return 0 ;
    }

    // Search for a specific value (num) using
    // linear search
    public int Search(int num){
	int j ;
	boolean ls01 ;
	int ifound ;
	int aux01 ;
	int aux02 ;
	int nt ;

	j = 1 ;
	ls01 = false ;
	ifound = 0 ;
	
	//System.out.println(num);
	while (j < (size)) {
	    aux01 = number[j] ;
	    aux02 = num + 1 ;
	    if (aux01 < num) nt = 0 ;
	    else if (!(aux01 < aux02)) nt = 0 ;
	    else {
		ls01 = true ;
		ifound = 1 ;
		j = size ;
	    }
	    j = j + 1 ;
	}

	return ifound ;
    }


    
    // initialize array of integers with some
    // some sequence
    public int Init(int sz){
	int j1 ;
	int k ;
	int aux011 ;
	int aux021 ;

	size = sz ;
	number = new int[sz] ;
	
	j1 = 1 ;
	k = size + 1 ;
	while (j1 < (size)) {
	    aux011 = 2 * j1 ;
	    aux021 = k - 3 ;
	    number[j1] = aux011 + aux021 ;
	    j1 = j1 + 1 ;
	    k = k - 1 ;
	}
	return 0 ;	
    }

}
