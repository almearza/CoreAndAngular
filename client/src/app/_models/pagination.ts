export interface Pagination {
    totalItems: number;
    totalPages: number;
    itemsPerPage: number;
    currentPage: number;
}
export class Paginatted<T>{
    result: T;
    pagination: Pagination;
}