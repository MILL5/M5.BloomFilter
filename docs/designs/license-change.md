# PolyForm Strict license adoption

The user selected PolyForm Strict 1.0.0 after a scan distinguished the recent
reused source-available license from separate proprietary MILL5 product terms.
The standard text matches Days on Purpose, Cut & Move, WordStake and MacEdgeLight.
Only its Required Notice is adapted to the existing MILL5 ownership:
Copyright (c) 2021-2026 MILL5, LLC.

Version 2.0.0 was already accepted by NuGet, so this change ships as 2.0.1.
The five active packages embed the exact root LICENSE through PackageLicenseFile;
all internal dependency versions advance together. Hash format remains 2.

README, site copy, FAQ structured data, discovery metadata and installation
commands identify the new version and source-available license. Third-party
Google EGM Apache and Barlow OFL licenses remain untouched. Separate proprietary
MILL5 product terms were not selected for BloomFilter.

Validation: compare the standard body byte-for-byte with the source template,
inspect all five packaged license files and nuspec metadata, run the existing
117 tests and a Release build, check license links and matching visible/structured
copy, then verify the release workflow's accepted uploads independently from
NuGet public validation/indexing and package restore.

Completed verification: all 117 tests passed in both Debug and Release; Release
build had zero warnings/errors. All five 2.0.1 archives contain the exact root
LICENSE and README and correct internal dependency versions. The license body
matches the reused WordStake template byte-for-byte apart from its Required
Notice; the wording matches the official standard. Bundled Apache/OFL license
files are byte-identical to the prior commit. A fresh consistency reviewer found
no material defects. Desktop/mobile license-copy acceptance passed.
