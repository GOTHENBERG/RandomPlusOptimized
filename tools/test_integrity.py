import sys
import os
import tempfile
import xml.etree.ElementTree as ET

sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "tools")))
import mcp_server

def run_tests():
    print("=== Testing MCP Integrity & Data Loss Guards ===")

    # Test 1: Real Assemblies PE header check
    ok16, msg16 = mcp_server.check_pe_header("Resources/1.6/Assemblies/RandomPlus.dll")
    ok17, msg17 = mcp_server.check_pe_header("Resources/1.7/Assemblies/RandomPlus.dll")
    print(f"[TEST 1] 1.6 Assembly PE Header: {'PASSED' if ok16 else 'FAILED'} ({msg16})")
    print(f"[TEST 1] 1.7 Assembly PE Header: {'PASSED' if ok17 else 'FAILED'} ({msg17})")
    assert ok16 and ok17, "Assemblies must be valid PE files"

    # Test 2: Fake Corrupt PE File
    with tempfile.NamedTemporaryFile(delete=False) as tf:
        tf.write(b"CORRUPTED_NON_PE_DATA_BYTES")
        tf_name = tf.name
    try:
        ok_corrupt, msg_corrupt = mcp_server.check_pe_header(tf_name)
        print(f"[TEST 2] Corrupt binary rejection: {'PASSED' if not ok_corrupt else 'FAILED'} ({msg_corrupt})")
        assert not ok_corrupt, "Corrupt file must be rejected"
    finally:
        os.remove(tf_name)

    # Test 3: XML & Localization Parity Check
    xml_ok, xml_msg = mcp_server.run_validate_xml()
    print(f"[TEST 3] XML & Localization Parity: {'PASSED' if xml_ok else 'FAILED'}")
    assert xml_ok, f"XML validation failed: {xml_msg}"

    # Test 4: Simulated Localization Data Loss (Missing key in Russian)
    ru_path = os.path.join("Resources", "Languages", "Russian", "Keyed", "RandomPlus.xml")
    with open(ru_path, "r", encoding="utf-8") as f:
        original_ru = f.read()

    # Tamper Russian file by removing one key
    tampered_ru = original_ru.replace("<RandomPlus.FilterButton>Фильтр</RandomPlus.FilterButton>", "")
    with open(ru_path, "w", encoding="utf-8") as f:
        f.write(tampered_ru)

    try:
        tampered_ok, tampered_msg = mcp_server.run_validate_xml()
        print(f"[TEST 4] Missing translation key detection: {'PASSED' if not tampered_ok else 'FAILED'}")
        assert not tampered_ok, "Missing key should trigger validation failure"
        assert "RandomPlus.FilterButton" in tampered_msg, "Must report exact missing key"
    finally:
        # Restore Russian file
        with open(ru_path, "w", encoding="utf-8") as f:
            f.write(original_ru)

    # Test 5: Simulated About.xml Corruption (Remove 1.7 version tag)
    about_path = os.path.join("Resources", "About", "About.xml")
    with open(about_path, "r", encoding="utf-8") as f:
        original_about = f.read()

    tampered_about = original_about.replace("<li>1.7</li>", "")
    with open(about_path, "w", encoding="utf-8") as f:
        f.write(tampered_about)

    try:
        about_ok, about_msg = mcp_server.run_validate_xml()
        print(f"[TEST 5] Missing 1.7 tag detection: {'PASSED' if not about_ok else 'FAILED'}")
        assert not about_ok, "Missing 1.7 tag must trigger validation failure"
        assert "1.7" in about_msg, "Must report missing 1.7"
    finally:
        with open(about_path, "w", encoding="utf-8") as f:
            f.write(original_about)

    # Test 6: Decompiler Artifact Guard
    bad_cs_path = os.path.join("Source", "BadArtifactTest.cs")
    with open(bad_cs_path, "w", encoding="utf-8") as f:
        f.write("namespace Test { class Bad { int CS$0001; } }\n")

    try:
        v_ok, v_msg = mcp_server.run_verify()
        print(f"[TEST 6] Decompiler artifact detection: {'PASSED' if not v_ok else 'FAILED'}")
        assert not v_ok, "Decompiler artifact must trigger failure"
        assert "CS$" in v_msg, "Must report CS$ artifact"
    finally:
        if os.path.isfile(bad_cs_path):
            os.remove(bad_cs_path)

    # Test 7: Final Clean Verification
    final_ok, final_msg = mcp_server.run_verify()
    print(f"[TEST 7] Clean State Full Verification: {'PASSED' if final_ok else 'FAILED'}")
    assert final_ok, "Final verification must pass"

    print("\n>>> ALL 7 INTEGRITY & DATA LOSS AUDIT TESTS PASSED SUCCESSFULLY! <<<")

if __name__ == "__main__":
    run_tests()
