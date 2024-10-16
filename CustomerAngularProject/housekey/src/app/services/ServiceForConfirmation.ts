import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PassTokenWithHeaderService } from './pass-token-with-header.service';
@Injectable({
        providedIn: 'root'
})
export class ServiceForConfirmation {
        private endPoint = "http://localhost:18105/api";
      
        constructor(private http: HttpClient,
                private PassTokenWithHeaderService: PassTokenWithHeaderService) { }
        
        getServiceName(id: number): Observable<any> {
                return this.http.get<any>(this.endPoint + "/Services/GetServiceNameByIDV2/" + id,
                        { headers: this.PassTokenWithHeaderService.getHeaders() });
        }
        getCustomerNameById(id: string): Observable<any> {
                return this.http.get<any>(this.endPoint + "/Customer/GetCustomerNameById/" + id,
                        { headers: this.PassTokenWithHeaderService.getHeaders() });
        }

}