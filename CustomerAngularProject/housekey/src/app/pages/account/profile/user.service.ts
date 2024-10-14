import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'http://localhost:18105/api/Customer/'; // استبدل هذا الرابط برابط API الخاص بك

  constructor(private http: HttpClient) { }

  getUserData(id:string): Observable<any> {
    return this.http.get(this.apiUrl + id); // تأكد من استخدام URL الصحيح
  }

  // إضافة دالة لتحديث بيانات العميل
 updateCustomerData(id: string, customerData: any): Observable<any> {
    return this.http.put(this.apiUrl + id, customerData); // تأكد من استخدام URL الصحيح
  }

  changePassword(id: string, data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}changePassword${id}`, data);
  }

  // user.service.ts
blockUser(id: string): Observable<any> {
  return this.http.put(`${this.apiUrl}block${id}`, null);
}

}
