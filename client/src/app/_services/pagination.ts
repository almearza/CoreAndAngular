import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { Paginatted } from "../_models/pagination";

export function getPaginattedHeaders(pageNumber:number,pageSize:number): HttpParams {
    let httpPrams = new HttpParams();
    httpPrams = httpPrams.append('pageNumber', pageNumber.toString());
    httpPrams = httpPrams.append('pageSize', pageSize.toString());
    return httpPrams;
  }
  export function getPaginattedResult<T>(http:HttpClient,url: string, httpPrams: HttpParams) {
    let paginattedResult: Paginatted<T> = new Paginatted();
    return http.get<T>(url, { observe: 'response', params: httpPrams }).pipe(
      map(response => {
        paginattedResult.result = response.body;
        let paginationHeader = response.headers.get('Pagination');
        if (paginationHeader != null) {
          paginattedResult.pagination = JSON.parse(paginationHeader);
        }
        return paginattedResult;
      })
    );
  }