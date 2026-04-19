# Slice Stage Goals for SA3D.Modeling Parity Inputs

Date: 2026-04-19
Status: Proposed

## Goal

Define the exact information SA3D.Modeling should emit so SoaSimMLD can compare behavior slice-by-slice for the SA3D port effort.

## Stage model

- **Slice 1**: primitive IO semantics and pointer/LUT behavior.
- **Slice 2**: NJ block map and metadata shell behavior.
- **Slice 3+**: node graph, attach/mesh, motion/animation, and normalized parity outputs.

---

## Slice 1 (Primitive foundations)

### Must emit

- Primitive read/write IO pairs:
  - integer/float/vector/color conversions,
  - address + imageBase context,
  - endianness context.
- BAMS conversion checkpoints:
  - raw value, converted radians/degrees, rounding boundaries.
- LUT behavior checkpoints:
  - add/get operations,
  - pointer identity hits/misses,
  - referenced type category.

### Why

All later slices rely on these low-level semantics. Drift here invalidates higher-level parity comparisons.

---

## Slice 2 (Container discovery)

### Must emit

- NJ block scan results:
  - block offset,
  - block header,
  - block size,
  - selected model/motion/texture block outcomes.
- Metadata shell parse summary:
  - block types encountered,
  - parsed entries count,
  - unknown block capture metadata.

### Why

Slice 2 establishes whether both implementations select the same data regions and metadata interpretation boundaries.

---

## Slice 3 (Node core graph)

### Must emit

- Node graph summary:
  - node count,
  - max depth,
  - parent/child/sibling integrity diagnostics.
- Per-node decode summary:
  - attributes,
  - transform mode (Euler/quaternion),
  - attach pointer presence.
- Pointer traversal checkpoints:
  - child and sibling pointer read decisions.

---

## Slice 4 (Attach dispatch + CHUNK container)

### Must emit

- Attach format dispatch decisions per node.
- CHUNK attach summary:
  - vertex chunk count,
  - poly chunk count,
  - bounds behavior,
  - weighted vs non-weighted indicators.

---

## Slice 5 (PolyChunk and strip semantics)

### Must emit

- PolyChunk type histogram.
- Strip decode summary:
  - strip count,
  - triangle attribute count,
  - degenerate strip handling metrics,
  - winding/reversal metrics.
- Material chunk sequence summary.

---

## Slice 6 (ModelFile NJ entry)

### Must emit

- Detection branch used (`SA` vs `NJ` path).
- NJ selected model block info.
- Model format decision (`BM`/`CM` lineage).
- End-to-end model summary envelope for fixture.

---

## Slice 7 (Motion/keyframe decode)

### Must emit

- Motion header summary:
  - node count,
  - keyframe type mask,
  - interpolation mode,
  - shortRot state.
- Per-channel keyframe counts by node index.
- Scalar/vector/quaternion channel decode checkpoints.

---

## Slice 8 (AnimationFile NJ entry)

### Must emit

- Animation detection branch used.
- Required fallback metadata decisions (e.g., nodeCount availability).
- Motion summary mapped into parity report semantic metrics.

---

## Slice 9 (Normalization parity outputs)

### Must emit

- Buffer mesh normalized summary:
  - vertex/corner/index totals,
  - stripified indicators,
  - material segmentation counts.
- Weighted mesh normalized summary:
  - mesh count,
  - root/dependency index sets,
  - normals/colors usage.
- Optional canonical signature per mesh/object for quick mismatch location.

---

## Cross-stage requirements

1. Deterministic output ordering and numeric formatting.
2. Stable operation identifiers for diagnostics correlation.
3. Stage-specific diagnostics in report (`code`, `message`, `stage`, `severity`).
4. Backward-compatible extension of parity report envelope.

