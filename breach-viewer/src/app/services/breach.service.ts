import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, throwError, tap } from 'rxjs';
import { Breach } from '../models/breach.model';

@Injectable({
  providedIn: 'root'
})
export class BreachService {
  private readonly apiUrl = 'http://localhost:5012/api/breach';

  constructor(private http: HttpClient) {}

  /**
   * Get breaches with optional date filtering
   * @param fromDate Optional start date for filtering
   * @param toDate Optional end date for filtering
   * @returns Observable of breach array
   */
  getBreaches(fromDate?: Date, toDate?: Date): Observable<Breach[]> {
    console.log('BreachService: Making API call to', this.apiUrl);
    
    let params = new HttpParams();
    
    if (fromDate) {
      params = params.set('fromDate', fromDate.toISOString());
      console.log('BreachService: Added fromDate param:', fromDate.toISOString());
    }
    
    if (toDate) {
      params = params.set('toDate', toDate.toISOString());
      console.log('BreachService: Added toDate param:', toDate.toISOString());
    }

    return this.http.get<Breach[]>(this.apiUrl, { params })
      .pipe(
        tap(response => {
          console.log('BreachService: API response received:', response?.length || 0, 'breaches');
        }),
        catchError(this.handleError)
      );
  }

  /**
   * Download PDF report of breaches
   * @param fromDate Optional start date for filtering
   * @param toDate Optional end date for filtering
   * @returns Observable of blob for PDF download
   */
  downloadBreachesPdf(fromDate?: Date, toDate?: Date): Observable<Blob> {
    console.log('BreachService: Making PDF API call to', `${this.apiUrl}/pdf`);
    
    let params = new HttpParams();
    
    if (fromDate) {
      params = params.set('fromDate', fromDate.toISOString());
    }
    
    if (toDate) {
      params = params.set('toDate', toDate.toISOString());
    }

    return this.http.get(`${this.apiUrl}/pdf`, { 
      params, 
      responseType: 'blob' 
    }).pipe(
      tap(response => {
        console.log('BreachService: PDF response received, size:', response.size);
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Handle HTTP errors
   * @param error The error response
   * @returns Observable that throws the error
   */
  private handleError(error: any): Observable<never> {
    console.error('BreachService: HTTP error occurred:', error);
    
    let errorMessage = 'An error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = error.error.message;
    } else {
      // Server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    
    console.error('BreachService: Error message:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
} 