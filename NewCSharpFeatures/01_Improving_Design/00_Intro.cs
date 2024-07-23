/*
 * A good design makes invalid programs "unrepresentable".
 * 
 * The early you get the error the better:
 *  - Type System & Analyzers (compilation time)
 *  - Unit Tests
 *  - Integration & End-2-End Tests
 *
 * Classical OO concepts:
 * - Precondition: argument validation.
 * - Postconditions: checking the results.
 * - Invariants: a statement that must be true at all (observable) times.
 * 
 * Meet classical FP concepts:
 * - Immutability makes invariants redundant. Construction preconditions are sufficient.
*/