import { Pipe, PipeTransform } from '@angular/core';

/// Maps a backend status to a brand status-chip class (SRD V11 §8.1 colours).
@Pipe({ name: 'statusChip' })
export class StatusChipPipe implements PipeTransform {
  transform(status: string | null | undefined): string {
    switch ((status ?? '').toLowerCase()) {
      case 'paid':
      case 'closed':
      case 'approved':
        return 'lex-status--success';
      case 'overdue':
      case 'cancelled':
      case 'rejected':
        return 'lex-status--alert';
      default:
        return 'lex-status--muted';
    }
  }
}
