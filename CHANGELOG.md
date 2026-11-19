# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

## Unreleased

### Added

### Fixed

## 2.26.4702

### Added

- ``Units.TryFindUnit(name, out unit)``: looks up a predefined unit and returns ``false`` for unknown names

### Changed

- an unknown ``+units`` value in a proj4 string now throws an ``ArgumentException`` (``Unknown unit: ...``).
  Before, it silently fell back to metres, which led to wrong coordinates (breaking change for strings with invalid units)
- unit names are matched case-insensitive (name, plural and abbreviation), e.g. ``+units=degrees``, ``+units=FT``

### Fixed

- the predefined units ``chain`` (``ch``), ``us-ch``, ``link`` and ``fath`` were defined, but not registered in ``Units.PredifinedUnits``,
  so ``+units=link`` etc. fell back to metres

## 2.25.1802

### Added

- Documentation: modification_de/en.md
- Debug transformation with cs2cs.core tool (see docs/debugging_de/en.md)

## 2.25.1501

### Added

- added support for *.gsb grid files
  
  - ``Proj4Net.Core.IO.Path.PROJ_LIB`` must be set to a path, where grid file is located
  - use paramter +nadgrids=filename.gsb
  - grids must be grids that transform from target to WGS84 (or some equivalent)

### Fixed

