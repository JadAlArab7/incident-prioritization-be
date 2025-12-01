-- Pre-chosen UUIDs for incident statuses
INSERT INTO incident.incident_statuses (id, code, name, name_ar) VALUES
  ('00000000-0000-0000-0000-00000000d001', 'draft', 'Draft', 'مسودة'),
  ('00000000-0000-0000-0000-00000000d002', 'in_review', 'In Review', 'قيد المراجعة'),
  ('00000000-0000-0000-0000-00000000d003', 'accepted', 'Accepted', 'مقبول'),
  ('00000000-0000-0000-0000-00000000d004', 'rejected', 'Rejected', 'مرفوض')
ON CONFLICT (code) DO NOTHING;