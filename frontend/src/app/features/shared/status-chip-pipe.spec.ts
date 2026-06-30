import { StatusChipPipe } from './status-chip-pipe';

describe('StatusChipPipe', () => {
  const pipe = new StatusChipPipe();

  it('maps paid/closed to success', () => {
    expect(pipe.transform('Paid')).toBe('lex-status--success');
    expect(pipe.transform('Closed')).toBe('lex-status--success');
  });

  it('maps overdue/cancelled to alert', () => {
    expect(pipe.transform('Overdue')).toBe('lex-status--alert');
    expect(pipe.transform('Cancelled')).toBe('lex-status--alert');
  });

  it('falls back to muted', () => {
    expect(pipe.transform('Draft')).toBe('lex-status--muted');
    expect(pipe.transform(null)).toBe('lex-status--muted');
  });
});
