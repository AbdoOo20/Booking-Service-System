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
        };

        setBanckAccount(id: string, banckAccount: string): Observable<any>{
                return this.http.post<any>(this.endPoint + "/Customer/SetBanckAccount/"+ id, banckAccount,
                        {headers: this.PassTokenWithHeaderService.getHeaders()}
                )
        };

}