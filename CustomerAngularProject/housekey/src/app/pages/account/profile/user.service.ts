import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { PassTokenWithHeaderService } from "@services/pass-token-with-header.service";

@Injectable({
  providedIn: "root",
})
export class UserService {
  private apiUrl = "http://lilynightapi.runasp.net/api/Customer/"; // استبدل هذا الرابط برابط API الخاص بك

  constructor(
    private http: HttpClient,
    public header: PassTokenWithHeaderService
  ) {}

  getUserData(id: string): Observable<any> {
    var headers = this.header.getHeaders();
    return this.http.get(this.apiUrl + id, { headers }); // تأكد من استخدام URL الصحيح
  }

  // إضافة دالة لتحديث بيانات العميل
  updateCustomerData(id: string, customerData: any): Observable<any> {
    var headers = this.header.getHeaders();
    return this.http.put(this.apiUrl + id, customerData, { headers }); // تأكد من استخدام URL الصحيح
  }

  changePassword(id: string, data: any): Observable<any> {
    var headers = this.header.getHeaders();
    return this.http.put(`${this.apiUrl}changePassword${id}`, data, {
      headers,
    });
  }

  // user.service.ts
  blockUser(id: string): Observable<any> {
    //var headers = this.header.getHeaders();
    var headers = {
      Authorization: `Bearer ${localStorage.getItem("token")}`,
      "Content-Type": "application/json",
    };
    console.log(headers); //
    return this.http.put(`${this.apiUrl}block/${id}`, null, { headers });
  }
}
