import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Category } from "../common/interfaces/category";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class CategoriesService {
  API_URL = "https://lilynightapi.runasp.net/api/Categories";
  constructor(private http: HttpClient) {}
  GetAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.API_URL);
  }
}
