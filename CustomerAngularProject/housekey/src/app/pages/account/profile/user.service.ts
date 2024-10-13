import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'http://localhost:5285/api/Customer/3'; // استبدل هذا الرابط برابط API الخاص بك

  constructor(private http: HttpClient) { }

  getUserData(): Observable<any> {
    return this.http.get(this.apiUrl); // تأكد من استخدام URL الصحيح
  }

 // editProfile(data: any): Observable<any> {
   // return this.http.put('http://localhost:5285/api/Customer/3', data);
 // }
  // إضافة دالة لتحديث بيانات العميل
 updateCustomerData(id: string, customerData: any): Observable<any> {
    return this.http.put('http://localhost:5285/api/Customer/3', customerData); // تأكد من استخدام URL الصحيح
  }

  changePassword(data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/change-password`, data);
  }

  // user.service.ts
blockUser(id: string): Observable<any> {
  return this.http.put(`${this.apiUrl}/block${2}`, null);
}

}
