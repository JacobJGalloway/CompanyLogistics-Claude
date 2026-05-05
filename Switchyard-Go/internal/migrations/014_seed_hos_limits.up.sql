-- HOSLimit seed data — initial operating states.
-- Add new states by inserting rows here; no code change required.
-- IL: 60/7 cycle only.
-- IN, WI: carrier may elect either 60/7 or 70/8 cycle — one row per cycle.
-- WI in-state extended limits (12hr driving / 16hr period) apply only when the
-- route remains entirely within Wisconsin. Federal defaults stored as base;
-- the HOS service must verify route geography before applying the in-state extension.

INSERT INTO hos_limit (
    id,
    state_code,
    cycle_label,
    daily_driving_limit_hours,
    daily_period_hours,
    rest_period_hours,
    weekly_limit_hours,
    weekly_period_days,
    weekly_reset_hours,
    sleeper_cab_min_hours,
    short_haul_radius_miles,
    adverse_weather_extension_hours,
    break_required_after_hours,
    effective_from,
    notes
) VALUES

-- Illinois — 60/7 only
(
    gen_random_uuid(), 'IL', '60/7',
    11, 14, 10, 60, 7, 34,
    7.0, 150, 2, 8, '2024-01-01',
    'Sleeper cab: 7-8 hrs min in cab during 10 hr rest. Short-haul (150 air mi): no ELD required if daily driving stays within 11 hr limit. Adverse weather: +2 hr driving extension within 14 hr on-duty window.'
),

-- Wisconsin — 60/7 cycle
(
    gen_random_uuid(), 'WI', '60/7',
    11, 14, 10, 60, 7, 34,
    7.0, 150, 2, 8, '2024-01-01',
    'Federal defaults. In-state routes only (all stops within WI): 12 hr driving / 16 hr on-duty period permitted. Short-haul (150 air mi): logging exempt if daily period stays within 14 hrs. Service must verify all stops are within WI before applying extended limits.'
),

-- Wisconsin — 70/8 cycle
(
    gen_random_uuid(), 'WI', '70/8',
    11, 14, 10, 70, 8, 34,
    7.0, 150, 2, 8, '2024-01-01',
    'Federal defaults. In-state routes only (all stops within WI): 12 hr driving / 16 hr on-duty period permitted. Short-haul (150 air mi): logging exempt if daily period stays within 14 hrs. Service must verify all stops are within WI before applying extended limits.'
),

-- Indiana — 60/7 cycle
(
    gen_random_uuid(), 'IN', '60/7',
    11, 14, 10, 60, 7, 34,
    6.5, 150, 2, 8, '2024-01-01',
    'Sleeper cab: 6.5 hrs min in cab during 10 hr rest (less than federal 7 hr minimum). Short-haul (150 air mi): logging exempt if daily period stays within 14 hrs.'
),

-- Indiana — 70/8 cycle
(
    gen_random_uuid(), 'IN', '70/8',
    11, 14, 10, 70, 8, 34,
    6.5, 150, 2, 8, '2024-01-01',
    'Sleeper cab: 6.5 hrs min in cab during 10 hr rest (less than federal 7 hr minimum). Short-haul (150 air mi): logging exempt if daily period stays within 14 hrs.'
);
