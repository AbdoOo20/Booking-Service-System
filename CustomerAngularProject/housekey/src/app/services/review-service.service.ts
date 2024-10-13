import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';


@Injectable({
  providedIn: 'root'
})
export class ReviewServiceService {
private readonly reviews_API="http://localhost:18105/api/Reviews";


  constructor(public http:HttpClient) { }

  getReview(bookid:number,customerId:string){
    return this.http.get(this.reviews_API+"/"+customerId+"/"+bookid);
  }
  postReview( postReview:any){
    return this.http.post(this.reviews_API,postReview)
  }
}
