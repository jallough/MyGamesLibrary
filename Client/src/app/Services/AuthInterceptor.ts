import { HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { HttpUserService } from './HttpUserService';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');
  const auth = inject(HttpUserService);
  const addAuthHeader = (r: HttpRequest<unknown>) => r.clone({ setHeaders: { Authorization: `Bearer ${localStorage.getItem('token')}` } });
  // If there's a token, clone the request and add the Authorization header
  if (token) {
    const cloned = addAuthHeader(req);
    return next(cloned).pipe(
    catchError((err: any) => {
      // if 401 try refresh once
      if (err?.status === 401) {
        return auth.refreshToken().pipe(
          switchMap(success => {
            if (success) {
              const retryReq = addAuthHeader(req);
              return next(retryReq);
            }
            return throwError(() => err);
          }),
          catchError(e => throwError(() => e))
        );
      }
      return throwError(() => err);
    })
  );
  }

  // Otherwise, pass the request through unchanged
  return next(req);
};